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
    public partial class Assessments : ContentPage
    {
        public Assessments()
        {
            InitializeComponent();
            AssessmentListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(AssessmentTapped);
            
            
        }
        public static string test;
        private async void ToAssessmentEditor_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AssessmentEditor());
        }
        private async void AssessmentTapped(object sender, ItemTappedEventArgs e)
        {
            if(CourseDetails.CourseAddAssessment)
            {
                var assessment = (AssessmentTable)e.Item;
                test = assessment.Type;
                if(assessment.Type == "Performance")
                {
                    await Terms.conn.ExecuteAsync("update Courses set PA = 1 where Id = " + CourseDetails.SelectedCourse.Id);
                    CourseDetails.SelectedCourse.PA = 1;
                } 
                else if(assessment.Type == "Objective")
                {
                    await Terms.conn.ExecuteAsync("update Courses set OA = 1 where Id = " + CourseDetails.SelectedCourse.Id);
                    CourseDetails.SelectedCourse.OA = 1;
                }
                await Terms.conn.ExecuteAsync("update Assessments set AssociatedCourse = " + CourseDetails.SelectedCourse.Id + " where Id = " + assessment.Id);
                CourseDetails.CourseAddAssessment = false;
                await Navigation.PopModalAsync();
            }
            else
            {
                var assessment = (AssessmentTable)e.Item;
                await Navigation.PushModalAsync(new AssessmentDetails(assessment));
            }
        }
        private async void CancelAdd_OnClick(object sender, EventArgs e)
        {
            CourseDetails.CourseAddAssessment = false;
            await Navigation.PopModalAsync();
        }
        protected override async void OnAppearing()
        {
            if(CourseDetails.CourseAddAssessment)
            {
                Cancel.IsVisible = true;
                ObservableCollection<AssessmentTable> unassignedAssessments;
                if(CourseDetails.SelectedCourse.OA == 0 && CourseDetails.SelectedCourse.PA == 0)
                {
                    var assessment = await Terms.conn.QueryAsync<AssessmentTable>("select * from Assessments where AssociatedCourse = 0");
                    unassignedAssessments = new ObservableCollection<AssessmentTable>(assessment);
                    AssessmentListView.ItemsSource = unassignedAssessments;
                } else if (CourseDetails.SelectedCourse.OA == 1 && CourseDetails.SelectedCourse.PA == 0)
                {
                    var assessment = await Terms.conn.QueryAsync<AssessmentTable>("select * from Assessments where AssociatedCourse = 0 and Type = \"Performance\"");
                    unassignedAssessments = new ObservableCollection<AssessmentTable>(assessment);
                    AssessmentListView.ItemsSource = unassignedAssessments;
                } else if (CourseDetails.SelectedCourse.OA == 0 && CourseDetails.SelectedCourse.PA == 1)
                {
                    var assessment = await Terms.conn.QueryAsync<AssessmentTable>("select * from Assessments where AssociatedCourse = 0 and Type = \"Objective\"");
                    unassignedAssessments = new ObservableCollection<AssessmentTable>(assessment);
                    AssessmentListView.ItemsSource = unassignedAssessments;
                }
                
               
            }
            else
            {
                Cancel.IsVisible = false;
                ObservableCollection<AssessmentTable> ListRefresh;
                var assessment = await Terms.conn.Table<AssessmentTable>().ToListAsync();
                ListRefresh = new ObservableCollection<AssessmentTable>(assessment);
                AssessmentListView.ItemsSource = ListRefresh;
            }
            
            base.OnAppearing();

        }
    }
}