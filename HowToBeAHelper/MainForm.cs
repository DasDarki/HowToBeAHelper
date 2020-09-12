using System.Windows.Forms;
using HowToBeAHelper.Properties;

namespace HowToBeAHelper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Text = Settings.Default.Title;
        }

        #region Seals

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        #endregion
    }
}
