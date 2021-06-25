using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;

namespace C971PA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseEditor : ContentPage
    {
        public CourseEditor()
        {
            InitializeComponent();
            
            var statusList = new List<string>();
            statusList.Add("In Progress");
            statusList.Add("Completed");
            statusList.Add("Dropped");
            statusList.Add("Plan To Take");
            
            StatusList.ItemsSource = statusList;
        }
        private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            EndDatePicker.MinimumDate = StartDatePicker.Date;
        }
        private void CancelButton_OnClick(object sender, EventArgs e)
        {
            CourseDetails.EditCourse = false;
            Navigation.PopModalAsync();
        }
        private async void SaveButton_OnClick(object sender, EventArgs e)
        {
            if (EntryValidation.IsEmptyEntry(NameEntry.Text) || StatusList.SelectedIndex == -1 || EntryValidation.IsEmptyEntry(InstNameEntry.Text) || EntryValidation.IsEmptyEntry(InstPhoneEntry.Text) || !EntryValidation.IsValidEmail(InstEmailEntry.Text))
            {
                await DisplayAlert("Invalid Entry", "One or more entries are invalid", "OK"); 
               
            } else
            {
                if (CourseDetails.EditCourse)
                {
                    CourseTable course = await (Terms.conn.Table<CourseTable>().Where(a => a.Id == CourseDetails.SelectedCourse.Id)).FirstOrDefaultAsync();
                    course.Name = NameEntry.Text;
                    course.Start = StartDatePicker.Date;
                    course.End = EndDatePicker.Date;
                    course.Status = StatusList.SelectedItem.ToString();
                    course.Instructor = InstNameEntry.Text;
                    course.Phone = InstPhoneEntry.Text;
                    course.Email = InstEmailEntry.Text;
                    course.Notes = NotesEntry.Text;
                    await Terms.conn.UpdateAsync(course);
                    CourseDetails.EditCourse = false;
                    CourseDetails.SelectedCourse = await (Terms.conn.Table<CourseTable>().Where(a => a.Id == CourseDetails.SelectedCourse.Id)).FirstOrDefaultAsync();
                }
                else
                {
                    var course = new CourseTable();
                    course.Name = NameEntry.Text;
                    course.Start = StartDatePicker.Date;
                    course.End = EndDatePicker.Date;
                    course.Status = StatusList.SelectedItem.ToString();
                    course.Instructor = InstNameEntry.Text;
                    course.Phone = InstPhoneEntry.Text;
                    course.Email = InstEmailEntry.Text;
                    course.Notes = NotesEntry.Text;
                    await Terms.conn.InsertAsync(course);
                }
                await Navigation.PopModalAsync();
            }
            
            
        }
        protected override void OnAppearing()
        {
            if(CourseDetails.EditCourse)
            {
                NameEntry.Text = CourseDetails.SelectedCourse.Name;
                StartDatePicker.Date = CourseDetails.SelectedCourse.Start;
                EndDatePicker.Date = CourseDetails.SelectedCourse.End;
                StatusList.SelectedItem = CourseDetails.SelectedCourse.Status;
                InstNameEntry.Text = CourseDetails.SelectedCourse.Instructor;
                InstPhoneEntry.Text = CourseDetails.SelectedCourse.Phone;
                InstEmailEntry.Text = CourseDetails.SelectedCourse.Email;
                NotesEntry.Text = CourseDetails.SelectedCourse.Notes;
            }
            base.OnAppearing();
        }
    }
}