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
    public partial class AssessmentEditor : ContentPage
    {
        public AssessmentEditor()
        {
            InitializeComponent();
            List<string> Type = new List<string>();
            Type.Add("Objective");
            Type.Add("Performance");
            TypePicker.ItemsSource = Type;
        }
        private async void CancelButton_OnClick(object sender, EventArgs e)
        {
            AssessmentDetails.EditAssessment = false;
            await Navigation.PopModalAsync();
        }
        private async void SaveButton_OnClick(object sender, EventArgs e)
        {
            if(EntryValidation.IsEmptyEntry(NameEntry.Text) || TypePicker.SelectedIndex == -1)
            {
                await DisplayAlert("Invalid Entry", "One or more entries are invalid.", "OK");
            } else
            {
                if(AssessmentDetails.EditAssessment)
                {
                    AssessmentTable assessment = await (Terms.conn.Table<AssessmentTable>().Where(a => a.Id == AssessmentDetails.SelectedAssessment.Id)).FirstOrDefaultAsync();
                    assessment.Name = NameEntry.Text;
                    assessment.Type = TypePicker.SelectedItem.ToString();
                    assessment.StartDate = StartDatePicker.Date;
                    assessment.EndDate = EndDatePicker.Date;
                    await Terms.conn.UpdateAsync(assessment);
                    AssessmentDetails.EditAssessment = false;
                    AssessmentDetails.SelectedAssessment = await (Terms.conn.Table<AssessmentTable>().Where(a => a.Id == AssessmentDetails.SelectedAssessment.Id)).FirstOrDefaultAsync();
                }
                else
                {
                    var assessment = new AssessmentTable();
                    assessment.Name = NameEntry.Text;
                    assessment.Type = TypePicker.SelectedItem.ToString();
                    assessment.StartDate = StartDatePicker.Date;
                    assessment.EndDate = EndDatePicker.Date;
                    await Terms.conn.InsertAsync(assessment);
                }
                await Navigation.PopModalAsync();
            }
        }
        private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            EndDatePicker.MinimumDate = StartDatePicker.Date;
        }

        protected override void OnAppearing()
        {
            if (AssessmentDetails.EditAssessment)
            {
                NameEntry.Text = AssessmentDetails.SelectedAssessment.Name;
                TypePicker.SelectedItem = AssessmentDetails.SelectedAssessment.Type;
                StartDatePicker.Date = AssessmentDetails.SelectedAssessment.StartDate;
                EndDatePicker.Date = AssessmentDetails.SelectedAssessment.EndDate;
            }
            base.OnAppearing();
        }
    }
}