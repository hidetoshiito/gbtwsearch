using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Boolean flag = true;
        // ★consumerKeyペアを設定する
        String appKey = "Dummy";
        String appSKey = "Dummy";

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private  void Form1_LoadAsync(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {

            txt_twsearch.ReadOnly = true;
            button1.Enabled = false;
            var keyword = txt_twsearch.Text;
            string battleid = "00000000";
            int cnt = 0;
            String[] star = {"★","｜","☆", "｜" };
            flag = true;

            try
            {
                var tokens = CoreTweet.Tokens.Create(appKey, appSKey, Properties.Settings.Default.usertoken, Properties.Settings.Default.usertokenS);
                while (flag)
                {
                    cnt++;
                    var result = new CoreTweet.SearchResult();
                    try
                    {
                        result = await tokens.Search.TweetsAsync(count => 1, q => keyword);
                    }
                    catch (Exception er)
                    {
                        Console.WriteLine(er.Message);
                        continue;
                    }
                    Console.WriteLine(result.Count);
                    foreach (var tweet in result)
                    {
                        string nowbattleid = "";
                        Console.WriteLine("{0}: {1}",tweet.CreatedAt, tweet.Text);
                        System.Text.RegularExpressions.Regex r =
                            new System.Text.RegularExpressions.Regex(
                                @"[a-fA-F0-9]{8,8}",
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                        System.Text.RegularExpressions.Match m = r.Match(tweet.Text);
                        while (m.Success)
                        {
                            nowbattleid = m.Value;
                            // 最初のヒットを利用
                            break;
                        }
                        if (battleid.Equals(nowbattleid))
                        {
                            break;
                        }
                        battleid = nowbattleid;
                        var jst = tweet.CreatedAt.LocalDateTime;

                        Clipboard.SetText(battleid);

                        textBox1.Text = jst + "\r\n" + tweet.User.ScreenName + "\r\n" + tweet.Text.Replace("\n", "\r\n").Replace("\r\r", "\r");

                        notifyIcon1.BalloonTipTitle = battleid;
                        notifyIcon1.BalloonTipText = tweet.Text;
                        notifyIcon1.ShowBalloonTip(1000);

                    }
                    await Task.Delay(Decimal.ToInt32(numericUpDown1.Value) * 250);
                    label3.Text = star[((cnt * 4 + 0) % 4)] + ":" + cnt;
                    await Task.Delay(Decimal.ToInt32(numericUpDown1.Value) * 250);
                    label3.Text = star[((cnt * 4 + 1) % 4)] + ":" + cnt;
                    await Task.Delay(Decimal.ToInt32(numericUpDown1.Value) * 250);
                    label3.Text = star[((cnt * 4 + 2) % 4)] + ":" + cnt;
                    await Task.Delay(Decimal.ToInt32(numericUpDown1.Value) * 250);
                    label3.Text = star[((cnt * 4 + 3) % 4)] + ":" + cnt;
                }
            }
            catch (CoreTweet.TwitterException er)
            {
                textBox1.Text = er.Message;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            flag = false;
            txt_twsearch.ReadOnly = false;
            button1.Enabled = true;
        }


        CoreTweet.OAuth.OAuthSession session;

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                session = CoreTweet.OAuth.Authorize(appKey, appSKey);
                var url = session.AuthorizeUri; // -> user open in browser
                textBox3.Text = "認証URLをクリップボードにコピーしました\r\n" +
                                "ブラウザのURLバーにctrl+vで貼り付け\r\n" +
                                "Twitter認証を行いPINコードを取得→入力→登録してください。\r\n" +
                                url.AbsoluteUri;
                Clipboard.SetText(url.AbsoluteUri);
            }
            catch (CoreTweet.TwitterException er)
            {
                textBox3.Text = "失敗！\r\n" + er.Message;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                var tokens = CoreTweet.OAuth.GetTokens(session, textBox2.Text);
                Properties.Settings.Default.usertoken = tokens.AccessToken;
                Properties.Settings.Default.usertokenS = tokens.AccessTokenSecret;
                Properties.Settings.Default.Save();
                textBox3.Text = "認証完了!";
            }
            catch (CoreTweet.TwitterException er)
            {
                textBox3.Text = "失敗！\r\n" + er.Message;
            }
            catch (System.Exception ex)
            {
                textBox3.Text = "エラー！\r\n" + ex.Message;
            }

        }
    }
}
