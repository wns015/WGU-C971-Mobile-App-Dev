using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Xamarin.Essentials;
using System.Collections.ObjectModel;

namespace C971PA
{
    [Table("Terms")]
    public class TermTable
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

    }
    [Table("Courses")]
    public class CourseTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int AssociatedTerm { get; set; }
        public string Status { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Instructor { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public int Notifications { get; set; }
        public int OA { get; set; }
        public int PA { get; set; }
    }
    [Table("Assessments")]
    public class AssessmentTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int AssociatedCourse { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Notifications { get; set; }

    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        
        public MainPage()
        {
            Terms terms = new Terms();
            Courses courses = new Courses();
            Assessments assessments = new Assessments();
            assessments.Title = "ASSESSMENTS";
            courses.Title = "COURSES";
            terms.Title = "TERMS";
            this.Children.Add(terms);
            this.Children.Add(courses);
            this.Children.Add(assessments);
            InitializeComponent();

            
        }

        
    }
}