using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.Collections.ObjectModel;
using Xamarin.Essentials;

namespace C971PA
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseDetails : ContentPage
    {
        public static CourseTable SelectedCourse;
        public static bool EditCourse;
        public ObservableCollection<AssessmentTable> AssessmentsInCourse;
        public static bool CourseAddAssessment;
        public static bool FromCourse;
        public CourseDetails(CourseTable course)
        {
            InitializeComponent();
            
            if(TermDetails.FromTerm == true)
            {
                RemoveCourse.IsVisible = true;
            }
            SelectedCourse = course;
            
            CourseAssessmentList.ItemTapped += new EventHandler<ItemTappedEventArgs>(AssessmentTapped);
        }

        private async void AssessmentTapped(object sender, ItemTappedEventArgs e)
        {
            FromCourse = true;
            AssessmentTable assessment = (AssessmentTable)e.Item;
            await Navigation.PushModalAsync(new AssessmentDetails(assessment));
        }

        private async void EditButton_OnClick(object sender, EventArgs e)
        {
            EditCourse = true;
            await Navigation.PushModalAsync(new CourseEditor());
        }
        private async void CloseButton_OnClick(object sender, EventArgs e)
        {
            TermDetails.FromTerm = false;
            await Navigation.PopModalAsync();
        }
        private async void DeleteButton_OnClick(object sender, EventArgs e)
        {
            TermDetails.FromTerm = false;
            bool choice = await DisplayAlert("Delete Confirmation", "Do you want to delete this course?", "Yes", "No");
            if(choice)
            {
                await Terms.conn.ExecuteAsync("update Assessments set AssociatedCourse = 0 where AssociatedCourse = " + SelectedCourse.Id);
                await Terms.conn.ExecuteAsync("delete from Courses where Id = " + SelectedCourse.Id);
                await Navigation.PopModalAsync();
            }
        }
        private async void Remove_OnClick(object sender, EventArgs e)
        {
            await Terms.conn.ExecuteAsync("update Courses set AssociatedTerm = 0 where Id = " + SelectedCourse.Id);
            TermDetails.FromTerm = false;
            await Navigation.PopModalAsync();
        }
        private async void AddAssessment_OnClick(object sender, EventArgs e)
        {
            CourseAddAssessment = true;
            await Navigation.PushModalAsync(new Assessments());
        }
        protected override async void OnAppearing()
        {
            CourseTitle.Text = SelectedCourse.Name;
            CourseStatus.Text = SelectedCourse.Status;
            CourseStart.Text = SelectedCourse.Start.ToString("MM:dd:yyyy");
            CourseEnd.Text = SelectedCourse.End.ToString("MM:dd:yyyy");
            InstName.Text = SelectedCourse.Instructor;
            InstPhone.Text = SelectedCourse.Phone;
            InstEmail.Text = SelectedCourse.Email;
            CourseNotes.Text = SelectedCourse.Notes;
            var courseAssessment = await Terms.conn.QueryAsync<AssessmentTable>("select * from Assessments where AssociatedCourse = " + SelectedCourse.Id);
            AssessmentsInCourse = new ObservableCollection<AssessmentTable>(courseAssessment);
            CourseAssessmentList.ItemsSource = AssessmentsInCourse;
            if(AssessmentsInCourse.Count == 2)
            {
                AssessmentAdd.IsVisible = false;
            } else if (AssessmentsInCourse.Count < 2)
            {
                AssessmentAdd.IsVisible = true;
            }
            if(EntryValidation.IsEmptyEntry(SelectedCourse.Notes))
            {
                ShareButton.IsVisible = false;
            }
            else
            {
                ShareButton.IsVisible = true;
            }
            if (SelectedCourse.Notifications == 1)
            {
                CourseNotification.IsToggled = true;
            }
            else if (SelectedCourse.Notifications == 0)
            {
                CourseNotification.IsToggled = false;
            }
            base.OnAppearing();
        }
        private async void OnToggle(object sender, ToggledEventArgs e)
        {
            if(CourseNotification.IsToggled)
            {
                await Terms.conn.ExecuteAsync("update Courses set Notifications = 1 where Id = " + SelectedCourse.Id);
                SelectedCourse.Notifications = 1;
            }
            else if (!CourseNotification.IsToggled)
            {
                await Terms.conn.ExecuteAsync("update Courses set Notifications = 0 where Id = " + SelectedCourse.Id);
                SelectedCourse.Notifications = 0;
            }
        }
        private async void ShareButton_OnClick(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = CourseNotes.Text,
                Title = "Share your notes for " + SelectedCourse.Name
            });
        }
    }
}