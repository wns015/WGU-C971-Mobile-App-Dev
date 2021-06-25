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
    public partial class Courses : ContentPage
    {
        public Courses()
        {
            InitializeComponent();
            CourseListView.ItemTapped += new EventHandler<ItemTappedEventArgs>(CourseTapped);
            
        }

        private async void ToCourseEditor_OnClick(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new CourseEditor());
        }

        protected override async void OnAppearing()
        {
            if (TermDetails.TermAddCourse == true)
            {
                Cancel.IsVisible = true;
                ObservableCollection<CourseTable> unassignedCourses;
                var course = await Terms.conn.QueryAsync<CourseTable>("select * from Courses where AssociatedTerm = 0");
                unassignedCourses = new ObservableCollection<CourseTable>(course);
                CourseListView.ItemsSource = unassignedCourses;

            } else
            {
                Cancel.IsVisible = false;
                ObservableCollection<CourseTable> ListRefresh;
                var course = await Terms.conn.Table<CourseTable>().ToListAsync();
                ListRefresh = new ObservableCollection<CourseTable>(course);
                CourseListView.ItemsSource = ListRefresh;
            }
            
            base.OnAppearing();
        }
        private async void CourseTapped(object sender, ItemTappedEventArgs e)
        {
            if(TermDetails.TermAddCourse == true)
            {
                var course = (CourseTable)e.Item;
                await Terms.conn.ExecuteAsync("update Courses set AssociatedTerm = " + TermDetails.SelectedTerm.Id + " where Id = " + course.Id);
                TermDetails.TermAddCourse = false;
                await Navigation.PopModalAsync();
            } else
            {
                var course = (CourseTable)e.Item;
                await Navigation.PushModalAsync(new CourseDetails(course));
            }
        }
        private async void CancelAdd_OnClick(object sender, EventArgs e)
        {
            TermDetails.TermAddCourse = false;
            await Navigation.PopModalAsync();
        }
    }
}