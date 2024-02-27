using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace FastColoredTextBoxNS
{
    class LogMsgItem
    {
        public string text;
        public Style style;
    }


    public class CCdFastColoredTextBoxLog : IDisposable
    {
        public static readonly TextStyle defaultStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular);
        public static readonly TextStyle warningStyle = new TextStyle(Brushes.BurlyWood, null, FontStyle.Regular);
        public static readonly TextStyle errorStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        public static readonly TextStyle headlineStyle = new TextStyle(Brushes.Black, null, FontStyle.Bold);


        FastColoredTextBoxNS.FastColoredTextBox fctb;
        Channel<LogMsgItem> _logMsgChannel = Channel.CreateUnbounded<LogMsgItem>();

        bool _isTextBoxBusy = false;
        CancellationTokenSource thradCancelSource = null;

        public CCdFastColoredTextBoxLog()
        {
            startLogMsgDisplayConsumeAsync();
        }


        public void Dispose()
        {
            this.fctb = null;

            closeQueueThread();
        }

        void closeQueueThread()
        {
            if (this.thradCancelSource != null)
            {
                this.thradCancelSource.Cancel();
                this.thradCancelSource = null;
            }
        }


        /// <summary>
        /// log를 표시할 TextBox 컨트롤을 연결함.
        /// </summary>
        /// <param name="fastColoredTextBox"></param>
        public void attachControl(FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBox)
        {
            this.fctb = fastColoredTextBox;
        }


        /// <summary>
        /// log를 깨끗하게 지움.
        /// </summary>
        public void clear()
        {
            this.fctb.Clear();
        }


        /// <summary>
        /// msg 로그를 기록함.
        /// </summary>
        /// <param name="msg"></param>
        public void log(string msg)
        {
            requestWriteLog(msg, CCdFastColoredTextBoxLog.defaultStyle);
        }

        public void warn(string msg)
        {
            requestWriteLog(msg, CCdFastColoredTextBoxLog.warningStyle);
        }

        public void err(string msg)
        {
            requestWriteLog(msg, CCdFastColoredTextBoxLog.errorStyle);
        }

        public void headline(string msg)
        {
            requestWriteLog(msg, CCdFastColoredTextBoxLog.headlineStyle);
        }


        /// <summary>
        /// TextBox에 로그를 출력함.
        /// </summary>
        /// <param name="fctb2"></param>
        /// <param name="text"></param>
        /// <param name="style"></param>
        private static void writeLogToTextBox(FastColoredTextBoxNS.FastColoredTextBox fctb2, List<LogMsgItem> logs)
        {
            if (fctb2 == null || logs == null)
                return;

            //some stuffs for best performance
            fctb2.BeginUpdate();
            // 여기서 이렇게 해줘야, 갱신이 안되고 빠르다.
            fctb2.Selection.BeginUpdate();
            //remember user selection
            var userSelection = fctb2.Selection.Clone();
            //add text with predefined style
            fctb2.TextSource.CurrentTB = fctb2;

            foreach (var log in logs)
            {
                if (log.style == null)
                    fctb2.AppendText(log.text, false);
                else
                    fctb2.AppendText(log.text, log.style, false);
            }

            fctb2.Invalidate();

            //restore user selection
            //if (!userSelection.IsEmpty || userSelection.Start.iLine < fctb2.LinesCount - 2)
            if (!userSelection.IsEmpty)
            {
                fctb2.Selection.Start = userSelection.Start;
                fctb2.Selection.End = userSelection.End;
            }
            else
                fctb2.GoEnd();//scroll to end of the text
                              //
            fctb2.Selection.EndUpdate();
            fctb2.EndUpdate();
        }


        /// <summary>
        /// 일정한계를 넘어가면, Text를 버림.
        /// </summary>
        void checkLimitedText()
        {
        }


        async Task startLogMsgDisplayConsumeAsync()
        {
            while (await _logMsgChannel.Reader.WaitToReadAsync())
            {
                if(_isTextBoxBusy)
                {
                    // 조금쉬어라.
                    Thread.Sleep(1);
                    continue;
                }

                // 여러개씩 모아서 출력함.
                int popCount = 0;
                var logs = new List<LogMsgItem>();
                while (_logMsgChannel.Reader.TryRead(out var item))
                {
                    logs.Add(item);
                    if (++popCount > 99)
                        break;
                }

                if (logs.Count > 0)
                {
                    _isTextBoxBusy = true;

                    if (logs != null && logs.Count > 0)
                    {
                        var act = new Action(() =>
                        {
                            try
                            {
                                writeLogToTextBox(this.fctb, logs);
                            }
                            finally
                            {
                                // 작업이 끝나면 반드시 되돌려야 함.
                                _isTextBoxBusy = false;
                            }
                        });

                        if (this.fctb.InvokeRequired)
                        {
                            this.fctb.BeginInvoke(act);
                        }
                        else
                        {
                            act();
                        }
                    }
                    else
                    {
                        // 작업이 끝나면 반드시 되돌려야 함.
                        _isTextBoxBusy = false;
                    }
                }
            }
        }


        public int getRemainingLogCount()
        {
            return this._logMsgChannel.Reader.Count;
        }


        /// <summary>
        /// 출력해야할 로그들을 실제로 출력함.
        /// </summary>
        /// <param name="count">한번에 출력할 로그갯수</param>
        public void flushLogs( int count = 0 )
        {
        }


        /// <summary>
        /// log 출력을 요청함.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="style"></param>
        private void requestWriteLog(string text, Style style)
        {
            // 무조건 Q에 넣음.
            // 나중에 Q가 해소되지 못했는데, 계속 쌓이는 상황은 그때 생각하자.
            var item = new LogMsgItem();
            item.text = text;
            item.style = style;

            this._logMsgChannel.Writer.WriteAsync(item);
        }
    }
}
