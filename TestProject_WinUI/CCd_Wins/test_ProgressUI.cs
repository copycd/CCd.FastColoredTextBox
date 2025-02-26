using CCd.Log;
using CCd.Wins.UI;




namespace TestProject_WinUI.CCd_Wins
{
    public class test_ProgressUI
    {
        [Fact]
        public void test_ProgressForm()
        {
            int checkCount = 0;
            // 10만개정도 메세지를 5~10초안에 출력해야 함.
            // 이것보다 느려지면, 먼가 변화가 생긴것임.
            int tryTotalCount = 100000;
            ProgressForm form = new ProgressForm(true);
            form.setUserJobFunc((progress, cancelToken) =>
            {
                progress.begin(tryTotalCount, null);
                for (int i = 0; i < tryTotalCount; i++)
                {
                    ++checkCount;
                    if (progress.step(i.ToString()) == false)
                        break;
                    progress.stepResult(CCd.Log.ResultType.success);
                    progress.msg(string.Format($"{i}:  This is Step and Message 메세지 입니다."));
                }

                progress.end();
                return true;
            });
            form.runAsync();

            // 끝날때가지 기다려야함.
            // 자동종료일 경우, log가 찍히는 속도가 못따라가서 찍히기전에 종료됨.
            Assert.True(checkCount == tryTotalCount);
        }


        /// <summary>
        /// 사용자가 End를 콜하지 않아도.
        /// Close 버튼이 활성화 되어야함.
        /// </summary>
        [Fact]
        public void test_ProgressForm_NoEnd()
        {
            int checkCount = 0;
            // TODO: 현재 작업은 끝나서 Close버튼이 활성화 되지만
            // 남아 있는 로그 메세지를 계속 출력하고 있는 문제가 있음.
            int tryTotalCount = 100000;
            ProgressForm form = new ProgressForm(false);
            form.setUserJobFunc((progress, cancelToken) =>
            {
                progress.begin(tryTotalCount, null);
                for (int i = 0; i < tryTotalCount; i++)
                {
                    ++checkCount;
                    if (progress.step(i.ToString()) == false)
                        break;
                    progress.stepResult(CCd.Log.ResultType.success);
                    progress.msg(string.Format($"{i}:  This is Step and Message 메세지 입니다."));
                }

                return true;
            });
            form.runAsync();
        }


        /// <summary>
        /// 너무 갑자기 많이할때, 멈춤이 발생하거나 갱신이 느려지면 안됨.
        /// 특히, index count는 이미 끝났는데, log message가 계속 표출되는지.
        /// </summary>
        [Fact]
        public void test_ProgressForm_Heavy()
        {
            int checkCount = 0;
            int tryTotalCount = 500000;
            ProgressForm form = new ProgressForm();
            form.setUserJobFunc((progress, cancelToken) =>
            {
                progress.begin(tryTotalCount, null);
                for (int i = 0; i < tryTotalCount; i++)
                {
                    ++checkCount;
                    if (progress.step(i.ToString()) == false)
                        break;
                    progress.stepResult(CCd.Log.ResultType.success);
                    progress.msg(string.Format($"{i}: Heavey Test : This is Step and Message 메세지 입니다."));
                }

                progress.end();
                return true;
            });
            form.runAsync();

            // 끝날때가지 기다려야함.
            // 자동종료일 경우, log가 찍히는 속도가 못따라가서 찍히기전에 종료됨.
            Assert.True(checkCount == tryTotalCount);
        }


        /// <summary>
        /// 한번 end하고 또 begin했을때 작동확인.
        /// </summary>
        [Fact]
        public void test_ProgressForm_StepReBegin()
        {
            int checkCount = 0;
            int tryStep1 = 2000;
            int tryStep2 = 20000;
            int tryTotalCount = tryStep1 + tryStep2;
            // autoClose를 하면, 2번째 BeginEnd가 실행이 안됨.
            ProgressForm form = new ProgressForm(false);
            form.setUserJobFunc((progress, cancelToken) =>
            {
                progress.begin(tryStep1, null);
                for (int i = 0; i < tryStep1; i++)
                {
                    ++checkCount;
                    if (progress.step(i.ToString()) == false)
                        break;
                    progress.stepResult(CCd.Log.ResultType.success);
                    progress.msg(string.Format($"{i}: First Message 입니다."));
                }

                progress.end();

                progress.begin(tryStep2, "두번째 Step 입니다.");
                for (int i = 0; i < tryStep2; i++)
                {
                    ++checkCount;
                    if (progress.step(i.ToString()) == false)
                        break;
                    progress.stepResult(CCd.Log.ResultType.success);
                    progress.msg(string.Format($"{i}: Second Message 입니다."));
                }

                progress.end();

                return true;
            });
            form.runAsync();

            // 끝날때가지 기다려야함.
            Assert.True(checkCount == tryTotalCount);
        }


        /// <summary>
        /// instant message가 얼마나 빠르게 처리가능한지 확인.
        /// </summary>
        [Fact]
        public void test_ProgressForm_InstantMsg()
        {
            int checkCount = 0;
            // 수치가 작으면, 너무 금방 끝남.
            // 2000만개 정도는 3~5초안에 끈나야함.
            // 이것보다 느려지면, 먼가 변화가 생긴것임.
            int tryTotalCount = 20000000;
            ProgressForm form = new ProgressForm(true);
            form.setUserJobFunc((progress, cancelToken) =>
            {
                progress.begin(tryTotalCount, null);
                for (int i = 0; i < tryTotalCount; i++)
                {
                    ++checkCount;
                    progress.msg(string.Format($"{i}:  이것은 Instant Message 입니다."), LogMsgType.instant);
                    if (progress.canceled())
                        break;
                }

                progress.end();
                return true;
            });
            form.runAsync();

            // 끝날때가지 기다려야함.
            Assert.True(checkCount == tryTotalCount);
        }

    }
}
