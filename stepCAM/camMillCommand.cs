using Rhino;
using Rhino.Commands;
using Rhino.UI;
using Eto.Drawing;
using Eto.Forms;
using System.ComponentModel;


namespace stepCAM
{


    public class Model : INotifyPropertyChanged
    {
        string finalDepth;
        string depthPerPass;
        string offsetDirection;

        public string FinalDepth
        {
            get { return finalDepth; }
            set
            {
                finalDepth = value;
                OnPropertyChanged("TotalDepth");
            }
        }

        public string DepthPerPass
        {
            get { return depthPerPass; }
            set
            {
                depthPerPass = value;
                OnPropertyChanged("DepthPerPass");
            }
        }

        public string OffsetDirection
        {
            get { return offsetDirection; }
            set
            {
                offsetDirection = value;
                OnPropertyChanged("OffsetDirection");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class camMill : Rhino.Commands.Command
    {
        public static Model myModel = new Model();

        public override string EnglishName
        {
            get { return "camMill"; }
        }

        protected override Result RunCommand(Rhino.RhinoDoc doc, RunMode mode)
        {
            /*
             * set the default variables
             */
            camMill.myModel.FinalDepth = "0";
            camMill.myModel.DepthPerPass = "0";
            camMill.myModel.OffsetDirection = "Inside";

            /*
             * display form and wait for the input
             */
            var form = new TestForm();
            form.RestorePosition();
            var dialog_rc = form.ShowModal(RhinoEtoApp.MainWindow);
            form.SavePosition();

            /*
             * process the input or quit
             */
            if (dialog_rc == Eto.Forms.DialogResult.Ok)
            {
                RhinoApp.WriteLine("Going to generate toolpath");
                RhinoApp.WriteLine("Offset Direction = {0}", myModel.OffsetDirection);
                RhinoApp.WriteLine("Final Depth = {0}", myModel.FinalDepth);
                RhinoApp.WriteLine("Depth per Pass = {0}", myModel.DepthPerPass);

            }
            else
            {
                RhinoApp.WriteLine("Command cancelled");
            }

            return Result.Success;
        }
    }


    public class TestForm : Dialog<DialogResult>
    {
        public TestForm()
        {
            Title = "Mill contour";
            var layout = new DynamicLayout();

            layout.Padding = new Padding(10);

            layout.BeginVertical(new Padding(10), new Size(10, 10));
            layout.AddRow(new Label { Text = "Offset direction:", TextAlignment = TextAlignment.Right }, fieldOffsetDirection());
            layout.AddRow(new Label { Text = "Final depth [mm]:", TextAlignment = TextAlignment.Right }, fieldFinalDepth());
            layout.AddRow(new Label { Text = "Depth per pass [mm]:", TextAlignment = TextAlignment.Right }, fieldDepthPerPass());
            layout.AddRow(null);
            layout.EndVertical();

            layout.AddSeparateRow(null, OkButton(), CancelButton(), null);

            this.Content = layout;

            // set the data context
            DataContext = camMill.myModel;
        }

        TextBox fieldFinalDepth()
        {
            var textBox = new TextBox();
            textBox.TextBinding.BindDataContext<Model>(r => r.FinalDepth);
            return textBox;
        }

        TextBox fieldDepthPerPass()
        {
            var textBox = new TextBox();
            textBox.TextBinding.BindDataContext<Model>(r => r.DepthPerPass);
            return textBox;
        }

        DropDown fieldOffsetDirection()
        {
            var dropDown = new DropDown { Items = { "Inside", "Outside" } };
            dropDown.SelectedKeyBinding.BindDataContext<Model>(r => r.OffsetDirection);
            return dropDown;
        }

        Button OkButton()
        {
            var button = new Button { Text = "OK" };

            button.Click += (sender, e) => {
                this.Result = Eto.Forms.DialogResult.Ok;
                this.Close();
            };

            return button;
        }

        Button CancelButton()
        {
            var button = new Button { Text = "Cancel" };

            button.Click += (sender, e) => {
                this.Result = Eto.Forms.DialogResult.Cancel;
                this.Close();
            };

            return button;
        }

    }
}
