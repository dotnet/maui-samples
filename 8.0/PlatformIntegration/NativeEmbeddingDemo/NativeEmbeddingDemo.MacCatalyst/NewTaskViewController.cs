namespace NativeEmbeddingDemo.MacCatalyst
{
    public class NewTaskViewController : UIViewController
    {
        UITextField taskTitleTextField = null;
        UITextField notesTextField = null;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View!.BackgroundColor = UIColor.SystemBackground;

            var stackView = new UIStackView
            {
                Axis = UILayoutConstraintAxis.Vertical,
                Alignment = UIStackViewAlignment.Fill,
                Distribution = UIStackViewDistribution.Fill,
                Spacing = 8,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            View.AddSubview(stackView);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                stackView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor, 20),
                stackView.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor, 20),
                stackView.TrailingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TrailingAnchor, -20),
                stackView.BottomAnchor.ConstraintLessThanOrEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20)
            });

            // Title text field
            taskTitleTextField = CreateTextField("Title");
            stackView.AddArrangedSubview(taskTitleTextField);

            // Notes text field
            notesTextField = CreateTextField("Notes");
            stackView.AddArrangedSubview(notesTextField);

            // Create task button
            var createTaskButton = new UIButton(UIButtonType.System);
            createTaskButton.SetTitle("Create Task", UIControlState.Normal);
            createTaskButton.TouchUpInside += CreateTaskButtonTapped;
            stackView.AddArrangedSubview(createTaskButton);
        }

        UITextField CreateTextField(string placeholder)
        {
            var uiTextField = new UITextField
            {
                Placeholder = placeholder,
                BorderStyle = UITextBorderStyle.RoundedRect,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            uiTextField.HeightAnchor.ConstraintEqualTo(40).Active = true;
            return uiTextField;
        }

        void CreateTaskButtonTapped(object? sender, EventArgs e)
        {
            Console.WriteLine("Create button tapped.");

            // Implement your logic here for creating a task.
        }
    }
}

