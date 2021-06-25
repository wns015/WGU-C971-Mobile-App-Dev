using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace C971PA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentDetails : ContentPage
    {
        public static AssessmentTable SelectedAssessment;
        public static bool EditAssessment;
        public AssessmentDetails(AssessmentTable assessment)
        {
            InitializeComponent();
            if(CourseDetails.FromCourse)
            {
                RemoveAssessment.IsVisible = true;
            }
            SelectedAssessment = assessment;
            
        }
        private async void OnToggle(object sender, ToggledEventArgs e)
        {
            if (AssessmentNotification.IsToggled)
            {
                await Terms.conn.ExecuteAsync("update Assessments set Notifications = 1 where Id = " + SelectedAssessment.Id);
                SelectedAssessment.Notifications = 1;
            }
            else if (!AssessmentNotification.IsToggled)
            {
                await Terms.conn.ExecuteAsync("update Assessments set Notifications = 0 where Id = " + SelectedAssessment.Id);
                SelectedAssessment.Notifications = 0;
            }
        }

        private async void EditButton_OnClick(object sender, EventArgs e)
        {
            EditAssessment = true;
            await Navigation.PushModalAsync(new AssessmentEditor());
        }

        private async void DeleteButton_OnClick(object sender, EventArgs e)
        {
            CourseDetails.FromCourse = false;
            bool choice = await DisplayAlert("Delete Confirmation", "Do you want to delete this assessment?", "Yes", "No");
            if(choice)
            {
                if(SelectedAssessment.AssociatedCourse != 0)
                {
                    if(SelectedAssessment.Type == "Performance")
                    {
                        await Terms.conn.ExecuteAsync("update Courses set PA = 0 where Id = " + SelectedAssessment.AssociatedCourse);
                        if(CourseDetails.FromCourse)
                        {
                            CourseDetails.SelectedCourse.PA = 0;
                        }
                        
                    } else if (SelectedAssessment.Type == "Objective")
                    {
                        await Terms.conn.ExecuteAsync("update Courses set OA = 0 where Id = " + SelectedAssessment.AssociatedCourse);
                        if (CourseDetails.FromCourse)
                        {
                            CourseDetails.SelectedCourse.OA = 0;
                        }
                    }
                    await Terms.conn.ExecuteAsync("delete from Assessments where Id = " + SelectedAssessment.Id);
                }
            }

        }
        private async void Remove_OnClick(object sender, EventArgs e)
        {
            if (SelectedAssessment.Type == "Performance")
            {
                await Terms.conn.ExecuteAsync("update Courses set PA = 0 where Id = " + SelectedAssessment.AssociatedCourse);
                CourseDetails.SelectedCourse.PA = 0;
            }
            else if (SelectedAssessment.Type == "Objective")
            {
                await Terms.conn.ExecuteAsync("update Courses set OA = 0 where Id = " + SelectedAssessment.AssociatedCourse);
                CourseDetails.SelectedCourse.OA = 0;
            }
            await Terms.conn.ExecuteAsync("update Assessments set AssociatedCourse = 0 where Id = " + SelectedAssessment.Id);
            CourseDetails.FromCourse = false;
            await Navigation.PopModalAsync();
        }

        private async void CloseButton_OnClick(object sender, EventArgs e)
        {
            CourseDetails.FromCourse = false;
            await Navigation.PopModalAsync();
        }
        protected override void OnAppearing()
        {
            AssessmentTitle.Text = SelectedAssessment.Name;
            StartDisplay.Text = SelectedAssessment.StartDate.ToString("MM/dd/yyyy");
            EndDisplay.Text = SelectedAssessment.EndDate.ToString("MM/dd/yyyy");
            Type.Text = SelectedAssessment.Type;
            if (SelectedAssessment.Notifications == 1)
            {
                AssessmentNotification.IsToggled = true;
            }
            else if (SelectedAssessment.Notifications == 0)
            {
                AssessmentNotification.IsToggled = false;
            }
            base.OnAppearing();
        }
    }
}