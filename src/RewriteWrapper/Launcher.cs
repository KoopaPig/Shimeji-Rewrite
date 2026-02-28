using System.Diagnostics;
using System.Drawing.Text;
using System.Media;
using System.Runtime.InteropServices;

namespace RewriteWrapper
{
    public partial class Launcher : Form
    {
        private System.Windows.Forms.Timer launchTimer;

        private int segaTimeMS = 3200;
        private int introTimeMS = 11000;
        private int disclaimerTimeMS = 25000;
        private int countdownTimeMS = 10000;
        private int fadeoutTimeMS = 1500;

        private PrivateFontCollection pfc = new PrivateFontCollection();

        private FadingPictureBox fadingPicture;

        private PictureBox introPicture;
        private Button launchButton;

        public Launcher()
        {
            #region SETUP

                this.Text = "Sonic.exe";
                this.Width = 750;
                this.Height = 772;
                this.BackColor = System.Drawing.Color.Black;

                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.MaximizeBox = false;
                this.MinimizeBox = true;

            #endregion SETUP

            StartSequence();
        }

        #region EVENTS
            

            private async void Event_SegaIntro()
            {
                introPicture = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                };
                introPicture.Image = RewriteWrapper.Properties.Resources.segastart;
                this.Controls.Add(introPicture);

                await Task.Delay(1800);
                introPicture.Image = RewriteWrapper.Properties.Resources.segaloop;

                SoundPlayer player = new SoundPlayer(RewriteWrapper.Properties.Resources.SegaScream);
                player.Play();
            }


            private void Event_SonicIntro()
            {
                SoundPlayer player = new SoundPlayer(RewriteWrapper.Properties.Resources.IntroJingle);
                player.Play();

                introPicture.Image = RewriteWrapper.Properties.Resources.SonicIntro;
            }
            
            private void Event_Disclaimer()
            {
                // Launch Button
                launchButton = new Button
                {
                    Text = "START",
                    Font = InitCustomFont(),
                    Dock = DockStyle.Bottom,
                    Height = 72,
                    BackColor = System.Drawing.Color.FromArgb(139, 0, 15),
                    ForeColor = System.Drawing.Color.White,
                    FlatStyle = FlatStyle.Flat,
                    UseCompatibleTextRendering = true
                };
                launchButton.FlatAppearance.BorderSize = 0;

                launchButton.Click += Event_LaunchJar;
                this.Controls.Add(launchButton);

                this.Controls.Remove(introPicture);
                fadingPicture = new FadingPictureBox
                {
                    Dock = DockStyle.Fill
                };
                this.Controls.Add(fadingPicture);

                // Load image from resources
                fadingPicture.SetImage(RewriteWrapper.Properties.Resources.Disclaimer);

                // Fade in over 2 seconds
                fadingPicture.Fade(true, 1000);

                SoundPlayer player = new SoundPlayer(RewriteWrapper.Properties.Resources.Warning);
                player.Play();
            }

            private async void Event_Countdown()
            {
                int secondsLeft = countdownTimeMS / 1000;

                while (secondsLeft > 0)
                {
                    launchButton.Text = $"{secondsLeft}";
                    await Task.Delay(1000);
                    secondsLeft--;
                }

                launchButton.Text = "TIME'S UP!";
                launchButton.Enabled = true;
            }

            private void Event_FadeOut()
            {
                fadingPicture.Fade(false, 1000);
            }

            private void Event_LaunchJar(object? obj, EventArgs? e)
            {
                // Get folder of the running EXE
                // (The "Rewrite Shimeji" folder should be in the same location as the exe)
                string exeFolder = AppDomain.CurrentDomain.BaseDirectory;

                var javaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jre", "bin", "javaw.exe");

                // Build path to JAR relative to EXE
                string jarPath = Path.Combine(exeFolder, "Rewrite Shimeji", "Shimeji-ee.jar");

            //var startInfo = new ProcessStartInfo
            //{
            //    FileName = javaPath,
            //    Arguments = "-jar \"" + jarPath + "\"",
            //    WorkingDirectory = System.IO.Path.GetDirectoryName(jarPath), // set to JAR folder
            //    UseShellExecute = false
            //};

            var startInfo = new ProcessStartInfo
            {
                FileName = "javaw", // rely on PATH
                Arguments = "-jar \"" + jarPath + "\"",
                WorkingDirectory = Path.GetDirectoryName(jarPath),
                UseShellExecute = false
            };


            Process.Start(startInfo);

                this.Close();
            }

            private async void StartSequence()
            {
                // Play sega intro
                Event_SegaIntro();
                await Task.Delay(segaTimeMS);
                
                // Play sonic intro
                Event_SonicIntro();

                // Activate disclaimer
                await Task.Delay(introTimeMS);
                Event_Disclaimer();

                // Wait to activate countdown
                await Task.Delay(disclaimerTimeMS);
                Event_Countdown();

                // Fade out after countdown
                await Task.Delay(countdownTimeMS);
                Event_FadeOut();
                
                // Launch jar after fade out
                await Task.Delay(fadeoutTimeMS);
                Event_LaunchJar(null, null);
            }
        #endregion


        #region HELPERS
            private Font InitCustomFont()
            {
                int fontLength = Properties.Resources.sonicCD.Length;

                // create a buffer to read in to
                byte[] fontdata = Properties.Resources.sonicCD;

                // create an unsafe memory block for the font data
                System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

                // copy the bytes to the unsafe memory block
                Marshal.Copy(fontdata, 0, data, fontLength);

                // pass the font to the font collection
                pfc.AddMemoryFont(data, fontLength);

                return new Font(pfc.Families[0], 16, FontStyle.Regular);
            }

        #endregion
    }
}