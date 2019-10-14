using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoboconTimer
{
    public partial class Form1 : Form
    {
        DateTime start_time;
        int timer_state = 0; // 0: 初期状態

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        int text_size;
        string display_str;
        int text_size_1 = 0;
        int text_size_2 = 0;
        bool sound_flag = false;


        private void timer1_Tick(object sender, EventArgs e)
        {
            // フォームの幅を取得
            int w = this.Width;
            text_size_1 = (int)(w * 0.15);
            text_size_2 = (int)(text_size_1 * 1.2);
            
            int game_time = int.Parse(txbGameTime.Text);
            int setting_time = int.Parse(txbSettingTime.Text);

            // 現在時間の取得
            DateTime now_time = DateTime.Now;


            lblSetting.Text = "";


            // カウント待機
            if (timer_state == 0)
            {
                display_str = "READY";
                text_size = text_size_1;
            }

            // スタートカウントダウン
            if (timer_state == 1)
            {
                long elapsed_sec = GetUnixTime(now_time) - GetUnixTime(start_time);
                int remaining_sec = 6 - (int)elapsed_sec;

                if (remaining_sec != 6)
                {
                    display_str = (remaining_sec).ToString();
                    text_size = text_size_2;
                }

                if (remaining_sec == 3 && sound_flag == false)
                {
                    sound_flag = true;
                    PlaySound("count");
                }

                if (remaining_sec == 0)
                {
                    timer_state++;
                }
            }

            // 「START」描画
            if (timer_state == 2)
            {
                sound_flag = false;

                long elapsed_sec = GetUnixTime(now_time) - GetUnixTime(start_time) - 6;

                display_str = "START";
                text_size = text_size_1;

                if (elapsed_sec > 1)
                {
                    timer_state++;
                }
            }

            // 経過時間の表示
            if (timer_state == 3)
            {
                long elapsed_sec = GetUnixTime(now_time) - GetUnixTime(start_time) - 6;
                int remaining_sec = 6 - (int)elapsed_sec;

                // 描画文字列の生成
                string min_str = (elapsed_sec / 60).ToString();
                string sec_str = (elapsed_sec % 60).ToString();

                // ゼロ埋め
                if (elapsed_sec % 60 < 10)
                {
                    sec_str = "0" + sec_str;
                }

                // 終了カウントダウン
                if (elapsed_sec >= game_time - 3)
                {
                    label1.ForeColor = Color.Yellow;
                    if (sound_flag == false)
                    {
                        sound_flag = true;
                        PlaySound("count");
                    }
                }

                // 試合時間終了後は赤
                if (elapsed_sec >= game_time)
                {
                    label1.ForeColor = Color.Red;
                }

                // 10分以上はカラム落ちするので、リセット
                if (elapsed_sec >= 600)
                {
                    reset();
                }

                display_str = min_str + ":" + sec_str;
                text_size = text_size_2;
            }

            // セッティングタイムカウント
            if (timer_state == 10)
            {
                long elapsed_sec = GetUnixTime(now_time) - GetUnixTime(start_time);

                lblSetting.Text = elapsed_sec.ToString();
                if (elapsed_sec == setting_time)
                {
                    PlaySound("whistle");
                    timer_state = 11;
                }
            }

            label1.Text = display_str;
            label1.Font = new System.Drawing.Font("DSEG14 Modern", text_size);
        }

        private void start()
        {
            PlaySound("startcall");

            // スタート時間を記録
            start_time = DateTime.Now;
            timer_state = 1;
        }

        private void setting()
        {
            PlaySound("whistle");

            // スタート時間を記録
            start_time = DateTime.Now;
            timer_state = 10;
        }

        private void whistle()
        {
            PlaySound("whistle");
        }

        private void reset()
        {
            timer_state = 0; 
            sound_flag = false;
            label1.ForeColor = Color.White;
            label1.Text = "READY";
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "s" || e.KeyCode.ToString() == "S")
            {
                start();
            }

            if (e.KeyCode.ToString() == "c" || e.KeyCode.ToString() == "C")
            {
                setting();
            }

            if (e.KeyCode.ToString() == "w" || e.KeyCode.ToString() == "W")
            {
                whistle();
            }

            if (e.KeyCode.ToString() == "r" || e.KeyCode.ToString() == "R")
            {
                reset();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            reset();
        }


        private void btnSetting_Click(object sender, EventArgs e)
        {
            setting();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            start();
        }

        private void btnWhistle_Click(object sender, EventArgs e)
        {
            whistle();
        }










        private System.Media.SoundPlayer player = null;
        //WAVEファイルを再生する
        private void PlaySound(string waveFileName)
        {
            //再生されているときは止める
            if (player != null)
                StopSound();

            //読み込む
            player = new System.Media.SoundPlayer(System.IO.Path.GetFullPath(".\\sound\\" + waveFileName + ".wav"));
            //非同期再生する
            player.Play();

            //次のようにすると、ループ再生される
            //player.PlayLooping();

            //次のようにすると、最後まで再生し終えるまで待機する
            //player.PlaySync();
        }

        //再生されている音を止める
        private void StopSound()
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
        }


        // UNIXエポックを表すDateTimeオブジェクトを取得
        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public static long GetUnixTime(DateTime targetTime)
        {
            // UTC時間に変換
            targetTime = targetTime.ToUniversalTime();

            // UNIXエポックからの経過時間を取得
            TimeSpan elapsedTime = targetTime - UNIX_EPOCH;

            // 経過秒数に変換
            return (long)elapsedTime.TotalSeconds;
        }
    }
}




