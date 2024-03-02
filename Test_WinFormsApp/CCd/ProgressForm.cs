using CCd.Log;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Channels;

// warning막음. xxx is only supported on: 'windows'.
#pragma warning disable CA1416

namespace CCd.Wins.UI
{
    [SupportedOSPlatform("windows")]
    public partial class ProgressForm : Form, ICCdProgress
    {
        class IndexStatus
        {
            public int currIndex = 0;
            public int skipCount = 0;
            public int totalCount = 0;
            public int failureCount = 0;
            public int warningCount = 0;
            public int successCount = 0;
            // 출력 메세지.
            public string text = "";

            public void reset(int totalCnt)
            {
                currIndex = 0;
                totalCount = totalCnt;
                failureCount = 0;
                warningCount = 0;
                successCount = 0;
                skipCount = 0;
            }
        }

        Task<bool> _userJobTask;
        CancellationTokenSource thradCancelSource = null;
        IndexStatus _indexStatus = new IndexStatus();
        Stopwatch _totalElapsedTimeWatch = new Stopwatch();
        ElapsedTimeCounter _timeCounter = new ElapsedTimeCounter();
        FastColoredTextBoxNS.CCdFastColoredTextBoxLog fastColoredTextBoxLog = new FastColoredTextBoxNS.CCdFastColoredTextBoxLog();
        bool contiuneProgress = true;
        bool started = false;
        long isIndexLabelCtrlBusy = 0;
        long _timeUpdatedTMsg = 0;
        bool writeInstantMsgToLog = false;

        // instant는 buffer의 크기를 많이 쌓을 필요가 없음.
        // 쌓이면, 이전에 쌓였던것 날려버리고, 새로운것을 무조건 기록해서 마지막것만 사용할것이므로.
        Channel<string> instantMsgChannel = Channel.CreateBounded<string>( new BoundedChannelOptions(2) { FullMode = BoundedChannelFullMode.DropOldest } );
        long isInstantLabelCtrlBusy = 0;
        bool _needToRefreshForm = false;

        // progress가 end면 자동 종료.
        bool autoCloseMode { get; set; } = false;

#if DEBUG
        int allowLogLevel = (int)LogMsgType.debug;
#else
        int allowLogLevel = (int)LogMsgType.system;
#endif

        public ProgressForm(bool autoClose = false)
        {
            InitializeComponent();

            // 메세지 처리하기전에 ctrl을 연결해줌.
            this.fastColoredTextBoxLog.attachControl(this.fastColoredTextBox1);

            startInstantMsgConsumeAsync();
#if DEBUG
            // end() 처리할때
            // this.label_InstantMsg.Text  에서자꾸 UI Thread 충돌 오류가 나서 해결이 안됨.
            // Invoke를 사용한 상황임. 그래서 어쩔수 없이 아래 설정을함.
            // Release일때는 발현되지 않음.
            CheckForIllegalCrossThreadCalls = false;
#endif

            this.autoCloseMode = autoClose;
            this.backgroundWorker_DisplayLog.WorkerSupportsCancellation = true;
        }


        public Task<bool> setUserTask(Func<ICCdProgress, CancellationToken, bool> userJob)
        {
            if (userJob != null)
            {
                this._userJobTask = createTask(userJob);
            }
            return this._userJobTask;
        }


        Task<bool> createTask(Func<ICCdProgress, CancellationToken, bool> userJob)
        {
            if (userJob == null)
                return null;

            this.thradCancelSource = new CancellationTokenSource();
            CancellationToken token = this.thradCancelSource.Token;

            try
            {
                // thread pool을 사용하지 않도록 함.
                var newTask = new Task<bool>(() => userJob(this, token), token, TaskCreationOptions.LongRunning);
                return newTask;
            }
            catch (OperationCanceledException ex)
            {
                msg(ex.Message, LogMsgType.error);
            }
            return null;
        }


        private void ProgressForm_Load(object sender, EventArgs e)
        {
            _totalElapsedTimeWatch.Start();
        }


        private void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopJobThread();
            this.fastColoredTextBoxLog.Dispose();
        }

        void printReport()
        {
            var t = TimeSpan.FromMilliseconds(_totalElapsedTimeWatch.ElapsedMilliseconds);
            var elapsedTimeMsg = "Total Elapsed Time : " + t.ToString(@"hh\:mm\:ss");
            msg(elapsedTimeMsg);
            msg(String.Format("Total Count : {0}", _indexStatus.totalCount));
            msg(String.Format("Success Count : {0}", _indexStatus.successCount));
            msg(String.Format("Failure Count : {0}", _indexStatus.failureCount));

            if (_indexStatus.warningCount > 0)
                msg(String.Format("Warning Count : {0}", _indexStatus.warningCount));
            if (_indexStatus.skipCount > 0)
                msg(String.Format("Skip Count : {0}", _indexStatus.skipCount));
        }


        void stopBackgroundWorker()
        {
            if (this.backgroundWorker_DisplayLog != null)
            {
                this.backgroundWorker_DisplayLog.CancelAsync();
                this.backgroundWorker_DisplayLog = null;
            }
        }


        void stopJobThread()
        {
            this.started = false;
            stopBackgroundWorker();

            try
            {
                this.contiuneProgress = false;
                if (this.thradCancelSource != null)
                {
                    this.thradCancelSource.Cancel();
                    this.thradCancelSource.Dispose();
                    this.thradCancelSource = null;
                }

                if (this._userJobTask != null)
                {
                    this._userJobTask.Dispose();
                    this._userJobTask = null;
                }
            }
            catch (Exception ex)
            {
                msg(ex.ToString(), LogMsgType.error);
            }
            finally
            {
                this.thradCancelSource = null;
                this._userJobTask = null;
            }
        }


        private void button_Cancel_Click(object sender, EventArgs e)
        {
            if(isDoing())
            {
                if (this._userJobTask.IsCompleted == false)
                {
                    if (MessageBox.Show("작업을 중지 하시겠습니까?", "", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        return;
                    stopJobThread();
                }

                msg("Canceled.", LogMsgType.warning);

                end();
            }
            else
            {
                Close();
            }
        }


        void updateProgressbar(IndexStatus indexStatus)
        {
            var per = (indexStatus.currIndex / (double)indexStatus.totalCount) * 100;
            int percent = (int)per;

            // 값이 0~100% 범위를 넘어가면 안됨.
            if (percent < 0)
                percent = 0;
            if (percent > 99)
                percent = 99;

            // 값에 변화가 있을때만 갱신하면됨.
            if (this.progressBar1.Value != percent)
            {
                var func = new Action(() =>
                {
                    this.progressBar1.Value = percent;
                });

                if (this.progressBar1.InvokeRequired)
                {
                    this.progressBar1.BeginInvoke(func);
                }
                else
                {
                    func();
                }
            }
        }

        void initProgressbar()
        {
            if (this.progressBar1.InvokeRequired)
            {
                Invoke(new Action(initProgressbar));
                return;
            }

            try
            {
                // 범위는 무조건 %이다.
                this.progressBar1.Minimum = 0;
                this.progressBar1.Maximum = 100;
                this.progressBar1.Step = 1;
                this.progressBar1.Value = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        void updateFailureLabel(IndexStatus indexStatus)
        {
            var func = new Action(() =>
            {
                this.label_FailureCount.Text = String.Format("Failure : {0}", indexStatus.failureCount);
            });
            
            if (this.label_FailureCount.InvokeRequired)
            {
                this.label_FailureCount.BeginInvoke(func);
            }
            else
            {
                func();
            }
        }


        void updateIndexLabel(IndexStatus indexStatus)
        {
            // UI가 바쁘면 쉬어가야함.
            if (Interlocked.Read(ref isIndexLabelCtrlBusy) == 1)
                return;

            var msg = string.Format("{0} / {1}", indexStatus.currIndex, indexStatus.totalCount);

            // 최소한 시간이 오래걸릴것 같은것들만 시간을 계산해서 보여주자.
            // 일단, 일정 갯수이상일때.
            if (indexStatus.totalCount > 50)
            {
                // 전체 대상 갯수를 파악할때, skip된것은 빼야함.
                var excludeCount = indexStatus.skipCount;
                // 전체 걸린시간대비 남은것 계산.
                var remainTime = RemainTimeUtil.calcRemianTime(_totalElapsedTimeWatch.ElapsedMilliseconds, indexStatus.currIndex - excludeCount, indexStatus.totalCount - excludeCount);
                // 일정시간 이상일때만 보여줌.
                if (remainTime > 60000)
                {
                    // 경과시간.
                    var tE = TimeSpan.FromMilliseconds(_totalElapsedTimeWatch.ElapsedMilliseconds);
                    // 전체 예상시간.
                    var totalTime = _totalElapsedTimeWatch.ElapsedMilliseconds + remainTime;
                    var tTotal = TimeSpan.FromMilliseconds(totalTime);
                    msg += String.Format("       {0} / {1}", tE.ToString(@"hh\:mm\:ss"), string.Format($"{tTotal.Hours}시{tTotal.Minutes}분"));
                }
            }

            // 완성 메세지가 변화가 없으면, 갱신하지 않음.
            if (indexStatus.text != msg)
            {
                indexStatus.text = msg;
                tryUpdateIndexLabel(msg);
            }
        }


        void tryUpdateIndexLabel(string msg)
        {
            Interlocked.Exchange(ref isIndexLabelCtrlBusy, 1);

            var func = new Action(() =>
            {
                try
                {
                    this.label_Index.Text = msg;
                }
                catch( Exception ex )
                {
                    Debug.WriteLine("tryUpdateIndexLabel:" + ex.Message);
                }
                finally
                {
                    Interlocked.Exchange(ref isIndexLabelCtrlBusy, 0);
                }
            });

            if (this.label_Index.InvokeRequired)
            {
                this.label_Index.BeginInvoke(func);
            }
            else
            {
                func();
            }
        }

        /// <summary>
        /// Instant Message가 있으면, 소비하는 task.
        /// </summary>
        /// <returns></returns>
        async Task startInstantMsgConsumeAsync()
        {
            while (await instantMsgChannel.Reader.WaitToReadAsync())
            {
                // 버퍼에 있는 마지막 놈만 출력하면 됨.
                string lastMsg = null;
                while (instantMsgChannel.Reader.TryRead(out var item))
                {
                    lastMsg = item;
                }

                if (lastMsg != null)
                {
                    tryUpdateInstantMsg(lastMsg);
                }
            }
        }


        void pushInstantMsg( string msg )
        {
            if (msg == null)
                return;
            instantMsgChannel.Writer.WriteAsync(msg);
        }


        void tryUpdateInstantMsg(string msg)
        {
            if (msg == null)
            {
                return;
            }

            // instant UI만 표출하는것이 아니고, log창에도 출력함.
            if (this.writeInstantMsgToLog)
            {
                this.fastColoredTextBoxLog.log(msg + Environment.NewLine);
                return;
            }

            // ui갱신명령이 수행됐다면.
            if (Interlocked.Read(ref isInstantLabelCtrlBusy) == 1)
            {
                // 아쉽지만, 해당 메세지는 소멸됨.
                return;
            }
            Interlocked.Exchange(ref isInstantLabelCtrlBusy, 1);

            var func = new Action(() =>
            {
                this.label_InstantMsg.Text = msg;
                Interlocked.Exchange(ref isInstantLabelCtrlBusy, 0);
            });

            if (this.label_InstantMsg.InvokeRequired)
            {
                this.label_InstantMsg.BeginInvoke(func);
            }
            else
            {
                func();
            }
        }


        public void runAsync()
        {
            this.backgroundWorker_DisplayLog?.RunWorkerAsync();

            if (this._userJobTask != null)
            {
                try
                {
                    this._userJobTask.Start();
                }
                catch (OperationCanceledException ex)
                {
                    msg(ex.Message, LogMsgType.error);
                }
            }

            this.ShowDialog();
        }


        public void runAsync(Func<ICCdProgress, CancellationToken, bool> userJob )
        {
            setUserTask(userJob);
            runAsync();
        }


        public bool begin(int totalCount, object tag)
        {
            this.started = true;
            _indexStatus.reset(totalCount);

            initProgressbar();

            this.fastColoredTextBoxLog.headline("BEGIN" + Environment.NewLine);
            return true;
        }

        /// <summary>
        /// progress를 한단계 나아가게 함.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public bool step(string sMsg = null, int totalCount = 0)
        {
            _timeCounter.beginStep();

            Interlocked.Increment(ref _indexStatus.currIndex);
            if (totalCount > 0)
                _indexStatus.totalCount = totalCount;

            updateIndexLabel(_indexStatus);
            updateProgressbar(_indexStatus);

            msg(sMsg, LogMsgType.step);

            // 여기서 false를 return하면, 사용자가 알아서 중단해야 함.
            return this.contiuneProgress;
        }


        /// <summary>
        /// step에 대한 결과를 기록함.
        /// 반드시 호출해야 하는건 아니지만, 나중에 통계를 작성할 수 있음.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="msg"></param>
        public void stepResult(CCd.Log.ResultType result, string sMsg = null)
        {
            switch (result)
            {
                case ResultType.warning:
                    Interlocked.Increment(ref _indexStatus.warningCount);
                    Interlocked.Increment(ref _indexStatus.successCount);
                    break;
                case ResultType.abort:
                case ResultType.success:
                    Interlocked.Increment(ref _indexStatus.successCount);
                    break;
                case ResultType.failure:
                    Interlocked.Increment(ref _indexStatus.failureCount);
                    updateFailureLabel(_indexStatus);
                    break;
                case ResultType.skip:
                    Interlocked.Increment(ref _indexStatus.skipCount);
                    Interlocked.Increment(ref _indexStatus.successCount);
                    break;
                default:
                    // 일단 skip으로 처리하자.
                    Interlocked.Increment(ref _indexStatus.skipCount);
                    break;
            }

            msg(sMsg, convertTo(result));

            // 일단은 성공인놈들만 시간을 모으자.
            if (result == ResultType.success)
                _timeCounter.endStep();
            else
                _timeCounter.endStep(true);
        }


        public bool canceled()
        {
            return !this.contiuneProgress;
        }


        LogMsgType convertTo(ResultType result)
        {
            switch (result)
            {
                case ResultType.failure:
                    return LogMsgType.error;
                case ResultType.warning:
                case ResultType.abort:
                    return LogMsgType.warning;
            }
            return LogMsgType.none;
        }


        // 작업중인지 판단.
        bool isDoing()
        {
            if (this.started)
                return true;
            if (this._userJobTask != null && this._userJobTask.IsCompleted == false)
                return true;
            return false;
        }


        public void end()
        {
            _indexStatus.currIndex = _indexStatus.totalCount;
            _timeCounter.endStep();

            if (this.started)
            {
                this.started = false;
                printReport();
            }

            updateFailureLabel(_indexStatus);

            this.fastColoredTextBoxLog.headline("END" + Environment.NewLine);

            Action safeUICall = delegate
            {
                this.button_Cancel.Text = "Close";
                this.label_InstantMsg.Text = "";
                this.Invalidate();
            };
            if (this.InvokeRequired)
            {
                this.Invoke(safeUICall);
            }
            else
            {
                safeUICall();
            }

            if (autoCloseMode)
            {
                flushEntireLog();

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        Close();
                    }));
                }
                else
                {
                    Close();
                }
            }
        }

        void flushEntireLog()
        {
            // 로그가 비워질때까지 기다리면 됨.
            // 로그출력은 내부 비동기로 작동함.
            while( this.fastColoredTextBoxLog.getLogStackedCount() > 0 )
            {
                Thread.Sleep(1);
            }
        }


        public void msg(string msg, LogMsgType type = LogMsgType.none)
        {
            if (type != LogMsgType.none)
            {
                // 허용한 type만 출력함.
                if ((this.allowLogLevel < (int)type))
                    return;
            }

            if (string.IsNullOrEmpty(msg))
                return;

            switch (type)
            {
                case LogMsgType.none:
                case LogMsgType.debug:
                    this.fastColoredTextBoxLog.log(msg + Environment.NewLine);
                    break;

                case LogMsgType.instant:
                case LogMsgType.step:
                    pushInstantMsg(msg);
                    break;

                case LogMsgType.warning:
                    this.fastColoredTextBoxLog.warn(msg + Environment.NewLine);
                    break;
                case LogMsgType.error:
                    this.fastColoredTextBoxLog.err(msg + Environment.NewLine);
                    break;
            }
        }


        private void backgroundWorker_DisplayLog_DoWork(object sender, DoWorkEventArgs e)
        {
            void tryUpdateCurrentIndexLabel()
            {
                if (_totalElapsedTimeWatch != null)
                {
                    // 확인차 확실하게 다시 뿌려주는것이므로 너무 빠르게 말고, 일정시간(3초)에 한번씩 갱신.
                    if (_totalElapsedTimeWatch.ElapsedMilliseconds - _timeUpdatedTMsg > 3000)
                    {
                        _timeUpdatedTMsg = _totalElapsedTimeWatch.ElapsedMilliseconds;
                        updateIndexLabel(_indexStatus);
                    }
                }
            }

            // 계속 메세지만을 뿌림.
            while (this.backgroundWorker_DisplayLog != null)
            {
                tryUpdateCurrentIndexLabel();
                Thread.Sleep(100);
            }
        }


        private void check_writeInstantMsg_CheckedChanged(object sender, EventArgs e)
        {
            this.writeInstantMsgToLog = this.check_writeInstantMsg.Checked;
        }
    }
}
