using FastColoredTextBoxNS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

// warning막음. xxx is only supported on: 'windows'.
#pragma warning disable CA1416


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
        ConcurrentQueue<LogMsgItem> msgQ = new ConcurrentQueue<LogMsgItem>();
        readonly object lockObj = new object();
        Boolean isBusy = false;
        CancellationTokenSource thradCancelSource = null;

        public CCdFastColoredTextBoxLog()
        {
        }


        public void Dispose()
        {
            this.isBusy = false;
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
        private static void logWrite(FastColoredTextBoxNS.FastColoredTextBox fctb2, List<LogMsgItem> logs)
        {
            if (fctb2 == null)
                return;

            //some stuffs for best performance
            fctb2.BeginUpdate();
            fctb2.Selection.BeginUpdate();
            //remember user selection
            var userSelection = fctb2.Selection.Clone();
            //add text with predefined style
            fctb2.TextSource.CurrentTB = fctb2;

            foreach (var log in logs)
            {
                if (log.style == null)
                    fctb2.AppendText(log.text);
                else
                    fctb2.AppendText(log.text, log.style);
            }

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


        /// <summary>
        /// Q에 내용이 있다면 하나를 수행함.
        /// </summary>
        List<LogMsgItem> popUpLogs(int popCount = 1)
        {
            if (this.msgQ.Count > 0)
            {
                try
                {
                    var result = new List<LogMsgItem>();
                    popCount = popCount > this.msgQ.Count ? this.msgQ.Count : popCount;
                    for (int i = 0; i < popCount; i++)
                    {
                        LogMsgItem item;
                        if (this.msgQ.TryDequeue(out item))
                        {
                            result.Add(item);
                        }
                        else
                        {
                            break;
                        }
                    }
                    return result;
                }
                catch( Exception ex )
                {
                    Debug.WriteLine(ex);
                }
            }
            return null;
        }


        public void flushLogs()
        {
            processLog();
        }



        /// <summary>
        /// 버퍼에 있는 내용을 출력시도함.
        /// </summary>
        void processLog()
        {
            // 바쁘면, 지나감.
            if (this.isBusy == false)
            {
                // 바쁘다고 표시하고,
                lock (lockObj)
                {
                    this.isBusy = true;
                }

                // 한번에 처리할 양만큼만 모음.
                List<LogMsgItem> logs = null;
                // 컨트롤이 있을때만 출력을 시도해야함.
                if (this.fctb != null)
                {
                    logs = popUpLogs(100);
                }

                if (logs != null && logs.Count > 0)
                {
                    var act = new Action(() =>
                    {
                        try
                        {
                            logWrite(this.fctb, logs);
                        }
                        finally
                        {
                            // 작업이 끝나면 반드시 되돌려야 함.
                            lock (lockObj)
                            {
                                this.isBusy = false;
                            }
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
                    lock (lockObj)
                    {
                        this.isBusy = false;
                    }
                }
            }
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

            this.msgQ.Enqueue(item);
        }
    }
}
