using System;

namespace CoolDudesFinal
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DatabaseAPI dbapi = new DatabaseAPI();
            dbapi.userLogin("oatmealJ@gmail.com", "oatmealJPassword", false);
            dbapi.userLogin("benS@CoolDudes.com", "benPassword", true);
            dbapi.userLogout();
            //dbapi.createStudent("Kirkland", "Pier", "kPier@gmail.com", "(555)555-8887", "kPierPassword");
            dbapi.userLogin("oatmealJ@gmail.com", "oatmealJPassword", false);
            //dbapi.viewAllClasses();
            //dbapi.viewEligible();
            //dbapi.viewPrereqsForClass("Intermediate Breakdancing");
            dbapi.userLogin("kPier@gmail.com", "kPierPassword", false);
            //dbapi.enrollInClass("Intro to Swing", "2020-03-02", 1900, "leader", true);
            dbapi.userLogin("orangeC@gmail.com", "orangeCPassword", false);
            //dbapi.dropClass("Intro to Foxtrot", "2020-03-07", 1000);
            //dbapi.viewAllInstructors();
            //dbapi.viewClassesByInstructor("Ben", "Smith");
            dbapi.viewAllStudents();
            dbapi.userLogin("benS@CoolDudes.com", "benPassword", true);
            //dbapi.viewAllStudents();
            //dbapi.viewStudentsPaidForClass("Intro to Foxtrot", "2020-03-07", 1000);
            //dbapi.viewStudentsNotPaidForClass("Intro to Foxtrot", "2020-03-07", 1000);
            //dbapi.viewAllStudentsInClass("Intro to Swing", "2020-03-02", 1900);
            //dbapi.viewAllLeadersInClass("Intro to Swing", "2020-03-02", 1900);
            //dbapi.viewAllFollowersInClass("Intro to Swing", "2020-03-02", 1900);

            ////test for add and remove instructor
            //dbapi.userLogin("joeS@CoolDudes.com", "joePassword", true);
            //dbapi.addInstructorToClass("Intro to Swing", "2020-03-02", 1900);
            //dbapi.viewInstructorsForClass("Intro to Swing", "2020-03-02", 1900);
            //dbapi.removeInstructorFromClass("Intro to Swing", "2020-03-02", 1900);
            //dbapi.viewInstructorsForClass("Intro to Swing", "2020-03-02", 1900);

            ////test for update bio
            //dbapi.displayInstructorBiography("Joe", "Smith");
            //dbapi.updateBiography("I teach dance");
            //dbapi.displayInstructorBiography("Joe", "Smith");
            //dbapi.updateBiography(null);
        }
    }
}
