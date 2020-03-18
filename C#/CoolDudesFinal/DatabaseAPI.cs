using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace CoolDudesFinal
{
    class DatabaseAPI
    {

        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private string currentUser = null;
        private bool isCurrentUserInstructor = false;
        public DatabaseAPI()
        {
            server = "localhost";
            database = "CoolDudesDanceStudio";
            uid = "root";
            password = "weAreSuperCool!";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                Console.WriteLine("Connection Open ! ");
                connection.Close();
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
            }
        }

        public void userLogin(string userEmail, string userPassword, bool instructor)
        {
            if (currentUser != null)
                currentUser = null;

            String table;
            if (instructor == true)
                table = "Instructor";
            else
                table = "Student";

            String queryText = "SELECT * FROM " + table + " WHERE emailAddress = '" + userEmail + "' AND pword = '" + userPassword + "';";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            String firstName = reader["firstName"].ToString();
            Console.WriteLine("Logged in as {0}", firstName);
            string email = reader["emailAddress"].ToString();
            currentUser = email;
            isCurrentUserInstructor = instructor;
            connection.Close();
        }

        public void createStudent(string firstName, string lastName, string emailAddress, string phoneNumber, string password)
        {
            if (currentUser != null)
            {
                Console.WriteLine("Cannot be logged in to create a new account.");
                return;
            }

            String queryText = "INSERT INTO Student(firstName, lastName, emailAddress, phoneNumber, pword)" +
                                $"VALUES('{firstName}', '{lastName}', '{emailAddress}', '{phoneNumber}', '{password}');";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;
            cmd.ExecuteNonQuery();
            connection.Close();

            Console.WriteLine($"Account for {firstName} {lastName} created.");
        }

        public void viewAllClasses()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            String queryText = "select title, cost, startDate, dayOfWeek, numOfWeeks, startTime, duration, roomNumber, capacity from class join classInstance on (class.id = classInstance.classid);";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("All classes:");
            Console.WriteLine("Title \t\t\t Cost \t Start Date \t\t Day \t Number of Weeks \t Start Time \t Duration \t Room Number \t Capacity");
            while (reader.Read())
            {
                String title = reader["title"].ToString();
                String cost = reader["cost"].ToString();
                String startDate = reader["startDate"].ToString();
                String day = reader["dayOfWeek"].ToString();
                String numWeeks = reader["numOfWeeks"].ToString();

                string startTime = reader["startTime"].ToString();
                Int32.TryParse(startTime, out int rawTime);
                int parsedHours = rawTime / 100;
                if (parsedHours > 12)
                    parsedHours -= 12;
                int parsedMinutes = rawTime % 100;
                string parsedTime = parsedHours + ":" + parsedMinutes;

                string duration = reader["Duration"].ToString();
                string roomNum = reader["roomNumber"].ToString();
                string capacity = reader["capacity"].ToString();

                Console.WriteLine($"{title} \t\t ${cost} \t {startDate} \t {day} \t {numWeeks} \t {parsedTime} \t {duration} \t {roomNum} \t {capacity}");
            }

            Console.WriteLine();
            connection.Close();
        }

        public void viewEligible()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            String queryText = "select title, cost, startDate, dayOfWeek, numOfWeeks, startTime, duration, roomNumber, capacity\n" +
                                "from class\n" +
                                    "join classinstance on(classinstance.classid = class.id)\n" +
                                "where class.id in (select prereqforid\n" +
                                                    "from prerequisite\n" +
                                                        "join class on (class.id = prerequisite.requiredid)\n" +
                                                    "where requiredid in (select class.id AS classId\n" +
                                                                        "from student\n" +
                                                                            "join enrolled on(enrolled.studentid = student.id)\n" +
                                                                            "join classinstance on(classinstance.id = enrolled.classinstanceid)\n" +
                                                                            "join class on (classinstance.classid = class.id)\n" +
                                                                        $"where emailAddress = '{currentUser}' AND enrolled.statusid = 2))\n" +
                                "UNION\n" +
                                "select title, cost, startDate, dayOfWeek, numOfWeeks, startTime, duration, roomNumber, capacity\n" +
                                "from class\n" +
                                    "join classinstance on (classinstance.classid = class.id)\n" +
                                    "left join prerequisite on (prerequisite.prereqforid = class.id)\n" +
                                "where prereqforid is null;";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("You are eligible for the following classes:");
            Console.WriteLine("Title \t\t\t Cost \t Start Date \t\t Day \t Number of Weeks \t Start Time \t Duration \t Room Number \t Capacity");
            while (reader.Read())
            {
                String title = reader["title"].ToString();
                String cost = reader["cost"].ToString();
                String startDate = reader["startDate"].ToString();
                String day = reader["dayOfWeek"].ToString();
                String numWeeks = reader["numOfWeeks"].ToString();

                string startTime = reader["startTime"].ToString();
                Int32.TryParse(startTime, out int rawTime);
                int parsedHours = rawTime / 100;
                if (parsedHours > 12)
                    parsedHours -= 12;
                int parsedMinutes = rawTime % 100;
                string parsedTime = parsedHours + ":" + parsedMinutes;

                string duration = reader["Duration"].ToString();
                string roomNum = reader["roomNumber"].ToString();
                string capacity = reader["capacity"].ToString();

                Console.WriteLine($"{title} \t\t ${cost} \t {startDate} \t {day} \t {numWeeks} \t {parsedTime} \t {duration} \t {roomNum} \t {capacity}");
            }

            Console.WriteLine();
            connection.Close();
        }

        public void viewPrereqsForClass(String classTitle)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            String queryText = "select title, cost, startDate, dayOfWeek, numOfWeeks, startTime, duration, roomNumber, capacity\n" +
                                "from class\n" +
                                    "join classinstance on(classinstance.classid = class.id)\n" +
                                "where class.id in (select requiredid\n" +
                                                    "from class\n" +
                                                        "join prerequisite on(prerequisite.prereqforid = class.id)\n" +
                                                    $"where title like '{classTitle}');\n";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("Title \t\t\t Cost \t Start Date \t\t Day \t Number of Weeks \t Start Time \t Duration \t Room Number \t Capacity");
            while (reader.Read())
            {
                String title = reader["title"].ToString();
                String cost = reader["cost"].ToString();
                String startDate = reader["startDate"].ToString();
                String day = reader["dayOfWeek"].ToString();
                String numWeeks = reader["numOfWeeks"].ToString();

                string startTime = reader["startTime"].ToString();
                Int32.TryParse(startTime, out int rawTime);
                int parsedHours = rawTime / 100;
                if (parsedHours > 12)
                    parsedHours -= 12;
                int parsedMinutes = rawTime % 100;
                string parsedTime = parsedHours + ":" + parsedMinutes;

                string duration = reader["Duration"].ToString();
                string roomNum = reader["roomNumber"].ToString();
                string capacity = reader["capacity"].ToString();

                Console.WriteLine($"{title} \t\t ${cost} \t {startDate} \t {day} \t {numWeeks} \t {parsedTime} \t {duration} \t {roomNum} \t {capacity}");
            }

            connection.Close();
        }

        public void enrollInClass(String classTitle, String classStartDate, int classStartTime, String role, bool payNow)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            int roleId;
            if (role == "leader")
                roleId = 3;
            else if (role == "follower")
                roleId = 2;
            else if (role == "solo")
                roleId = 1;
            else
            {
                Console.WriteLine("Invalid role");
                return;
            }

            int payId;
            if (payNow)
                payId = 1;
            else
                payId = 0;

            String queryText = "insert into enrolled\n" +
                                "values((select id\n" +
                                        "from student\n" +
                                        $"where emailAddress like '{currentUser}'),\n" +
                                        "(select classinstance.id\n" +
                                        "from class\n" +
                                            "join classinstance on(classinstance.classid = class.id)\n" +
		                                $"where class.title like '{classTitle}' AND startTime = {classStartTime} AND startDate = date('{classStartDate}')),\n" +
                                        $"{roleId},\n" +
                                        $"{payId},\n" +
                                        "1);";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;
            cmd.ExecuteNonQuery();
            connection.Close();

            Console.WriteLine($"Successfully enrolled in {classTitle} starting {classStartDate}.");
        }

        public void dropClass(String classTitle, String classStartDate, int classStartTime)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            String queryText = "update enrolled\n" +
                                "set statusId = 3\n" +
                                "where studentid = (select id\n" +
                                        "from student\n" +
                                        $"where emailAddress like '{currentUser}')\n" +
                                        "AND\n" +
                                        "classinstanceid = (select classinstance.id\n" +
                                                            "from class\n" +
                                                                "join classinstance on(classinstance.classid = class.id)\n" +
                                                            $"where class.title like '{classTitle}' AND startTime = {classStartTime} AND startDate = date('{classStartDate}'));";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void viewAllInstructors()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            String queryText = "select firstName, lastName, phoneNumber, emailAddress, yearsExperience\n" +
                                "from instructor;";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("First Name \t Last Name \t Phone Number \t Email Address \t Years Experience");
            while (reader.Read())
            {
                String firstName = reader["firstName"].ToString();
                String lastName = reader["lastName"].ToString();
                String phoneNumber = reader["phoneNumber"].ToString();
                String emailAddress = reader["emailAddress"].ToString();
                String yearsExperience = reader["yearsExperience"].ToString();

                Console.WriteLine($"{firstName} \t {lastName} \t {phoneNumber} \t {emailAddress} \t {yearsExperience}");
            }

            connection.Close();
        }

        public void viewClassesByInstructor(String firstName, String lastName)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            String queryText = "select title, cost, startDate, dayOfWeek, numOfWeeks, startTime, duration, roomNumber, capacity\n" +
                                "from instructor\n" +
                                    "join teaching on (teaching.instructorid = instructor.id)\n" +
                                    "join classinstance on (teaching.classinstanceid = classinstance.id)\n" +
                                    "join class on (classinstance.classid = class.id)\n" +
                                $"where firstName like '{firstName}' AND lastName like '{lastName}';";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("Title \t\t\t Cost \t Start Date \t\t Day \t Number of Weeks \t Start Time \t Duration \t Room Number \t Capacity");
            while (reader.Read())
            {
                String title = reader["title"].ToString();
                String cost = reader["cost"].ToString();
                String startDate = reader["startDate"].ToString();
                String day = reader["dayOfWeek"].ToString();
                String numWeeks = reader["numOfWeeks"].ToString();

                string startTime = reader["startTime"].ToString();
                Int32.TryParse(startTime, out int rawTime);
                int parsedHours = rawTime / 100;
                if (parsedHours > 12)
                    parsedHours -= 12;
                int parsedMinutes = rawTime % 100;
                string parsedTime = parsedHours + ":" + parsedMinutes;

                string duration = reader["Duration"].ToString();
                string roomNum = reader["roomNumber"].ToString();
                string capacity = reader["capacity"].ToString();

                Console.WriteLine($"{title} \t\t ${cost} \t {startDate} \t {day} \t {numWeeks} \t {parsedTime} \t {duration} \t {roomNum} \t {capacity}");
            }

            connection.Close();
        }

        public void viewAllStudents()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            if (!isCurrentUserInstructor)
            {
                Console.WriteLine("This function is not available to students.");
                return;
            }

            String queryText = "select firstName, lastName, emailAddress, phoneNumber\n" +
                                "from student;";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("First Name \t Last Name \t Email Address \t Phone Number");
            while (reader.Read())
            {
                String firstName = reader["firstName"].ToString();
                String lastName = reader["lastName"].ToString();
                String phoneNumber = reader["phoneNumber"].ToString();
                String emailAddress = reader["emailAddress"].ToString();

                Console.WriteLine($"{firstName} \t {lastName} \t {emailAddress} \t {phoneNumber}");
            }

            connection.Close();
        }

        public void viewStudentsPaidForClass(String classTitle, String classStartDate, int classStartTime)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            if (!isCurrentUserInstructor)
            {
                Console.WriteLine("This function is not available to students.");
                return;
            }

            String queryText = "select firstName, lastName, emailAddress, phoneNumber\n" +
                                "from class\n" +
                                    "join classinstance on(classinstance.classid = class.id)\n" +
                                    "join enrolled on(classinstance.id = enrolled.classinstanceid)\n" +
                                    "join student on(student.id = enrolled.studentid)\n" +
                                $"where class.title like '{classTitle}' AND startTime = {classStartTime} AND startDate = date('{classStartDate}') AND paid = 1;";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("First Name \t Last Name \t Email Address \t Phone Number");
            while (reader.Read())
            {
                String firstName = reader["firstName"].ToString();
                String lastName = reader["lastName"].ToString();
                String phoneNumber = reader["phoneNumber"].ToString();
                String emailAddress = reader["emailAddress"].ToString();

                Console.WriteLine($"{firstName} \t {lastName} \t {emailAddress} \t {phoneNumber}");
            }

            connection.Close();
        }

        public void viewStudentsNotPaidForClass(String classTitle, String classStartDate, int classStartTime)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            if (!isCurrentUserInstructor)
            {
                Console.WriteLine("This function is not available to students.");
                return;
            }

            String queryText = "select firstName, lastName, emailAddress, phoneNumber\n" +
                                "from class\n" +
                                    "join classinstance on(classinstance.classid = class.id)\n" +
                                    "join enrolled on(classinstance.id = enrolled.classinstanceid)\n" +
                                    "join student on(student.id = enrolled.studentid)\n" +
                                $"where class.title like '{classTitle}' AND startTime = {classStartTime} AND startDate = date('{classStartDate}') AND paid = 0;";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("First Name \t Last Name \t Email Address \t Phone Number");
            while (reader.Read())
            {
                String firstName = reader["firstName"].ToString();
                String lastName = reader["lastName"].ToString();
                String phoneNumber = reader["phoneNumber"].ToString();
                String emailAddress = reader["emailAddress"].ToString();

                Console.WriteLine($"{firstName} \t {lastName} \t {emailAddress} \t {phoneNumber}");
            }

            connection.Close();
        }

        public void viewAllStudentsInClass(String classTitle, String classStartDate, int classStartTime)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            if (!isCurrentUserInstructor)
            {
                Console.WriteLine("This function is not available to students.");
                return;
            }

            String queryText = "select firstName, lastName, emailAddress, phoneNumber\n" +
                                "from class\n" +
                                    "join classinstance on(classinstance.classid = class.id)\n" +
                                    "join enrolled on(enrolled.classinstanceid = classinstance.id)\n" +
                                    "join student on(student.id = enrolled.studentid)\n" +
                                $"where class.title like '{classTitle}' AND startTime = {classStartTime} AND startDate = date('{classStartDate}') AND statusId = 1;";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("First Name \t Last Name \t Email Address \t Phone Number");
            while (reader.Read())
            {
                String firstName = reader["firstName"].ToString();
                String lastName = reader["lastName"].ToString();
                String phoneNumber = reader["phoneNumber"].ToString();
                String emailAddress = reader["emailAddress"].ToString();

                Console.WriteLine($"{firstName} \t {lastName} \t {emailAddress} \t {phoneNumber}");
            }

            connection.Close();
        }

        public void viewAllLeadersInClass(String classTitle, String classStartDate, int classStartTime)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            if (!isCurrentUserInstructor)
            {
                Console.WriteLine("This function is not available to students.");
                return;
            }

            String queryText = "select firstName, lastName, emailAddress, phoneNumber\n" +
                                "from class\n" +
                                    "join classinstance on(classinstance.classid = class.id)\n" +
                                    "join enrolled on(enrolled.classinstanceid = classinstance.id)\n" +
                                    "join student on(student.id = enrolled.studentid)\n" +
                                $"where class.title like '{classTitle}' AND startTime = {classStartTime} AND startDate = date('{classStartDate}') AND statusId = 1 AND roleID = 3;";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("First Name \t Last Name \t Email Address \t Phone Number");
            while (reader.Read())
            {
                String firstName = reader["firstName"].ToString();
                String lastName = reader["lastName"].ToString();
                String phoneNumber = reader["phoneNumber"].ToString();
                String emailAddress = reader["emailAddress"].ToString();

                Console.WriteLine($"{firstName} \t {lastName} \t {emailAddress} \t {phoneNumber}");
            }

            connection.Close();
        }

        public void viewAllFollowersInClass(String classTitle, String classStartDate, int classStartTime)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            if (!isCurrentUserInstructor)
            {
                Console.WriteLine("This function is not available to students.");
                return;
            }

            String queryText = "select firstName, lastName, emailAddress, phoneNumber\n" +
                                "from class\n" +
                                    "join classinstance on(classinstance.classid = class.id)\n" +
                                    "join enrolled on(enrolled.classinstanceid = classinstance.id)\n" +
                                    "join student on(student.id = enrolled.studentid)\n" +
                                $"where class.title like '{classTitle}' AND startTime = {classStartTime} AND startDate = date('{classStartDate}') AND statusId = 1 AND roleID = 2;";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("First Name \t Last Name \t Email Address \t Phone Number");
            while (reader.Read())
            {
                String firstName = reader["firstName"].ToString();
                String lastName = reader["lastName"].ToString();
                String phoneNumber = reader["phoneNumber"].ToString();
                String emailAddress = reader["emailAddress"].ToString();

                Console.WriteLine($"{firstName} \t {lastName} \t {emailAddress} \t {phoneNumber}");
            }

            connection.Close();
        }

        public void addInstructorToClass(String classTitle, String classStartDate, int classStartTime)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            if (!isCurrentUserInstructor)
            {
                Console.WriteLine("This function is not available to students.");
                return;
            }

            String queryText = "insert into teaching\n" +
                                "values((select id\n" +
                                        "from instructor\n" +
                                        $"where emailAddress like '{currentUser}'),\n" +
                                        "(select classinstance.id\n" +
                                        "from classinstance\n" +
                                            "join class on (class.id = classinstance.classid)\n" +
		                                $"where class.title like '{classTitle}' AND startTime = {classStartTime} AND startDate = date('{classStartDate}')));";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void removeInstructorFromClass(String classTitle, String classStartDate, int classStartTime)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            if (!isCurrentUserInstructor)
            {
                Console.WriteLine("This function is not available to students.");
                return;
            }

            String queryText = "delete from teaching\n" +
                                "where instructorid = (select id\n" +
                                                    "from instructor\n" +
                                                    $"where emailAddress like '{currentUser}')\n" +
                                    "AND classinstanceid = (select classinstance.id\n" +
                                                            "from classinstance\n" +
                                                                "join class on (class.id = classinstance.classid)\n" +
                                                            $"where class.title like '{classTitle}' AND startTime = {classStartTime} AND startDate = date('{classStartDate}'));";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void viewInstructorsForClass(String classTitle, String classStartDate, int classStartTime)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            String queryText = "select firstName, lastName, phoneNumber, emailAddress, yearsExperience\n" +
                                "from instructor\n" +
                                    "join teaching on (teaching.instructorid = instructor.id)\n" +
                                    "join classinstance on (teaching.classinstanceid = classinstance.id)\n" +
                                    "join class on (class.id = classinstance.id)\n" +
                                $"where class.title like '{classTitle}' AND startTime = {classStartTime} AND startDate = date('{classStartDate}');";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("First Name \t Last Name \t Phone Number \t Email Address \t Years Experience");
            while (reader.Read())
            {
                String firstName = reader["firstName"].ToString();
                String lastName = reader["lastName"].ToString();
                String phoneNumber = reader["phoneNumber"].ToString();
                String emailAddress = reader["emailAddress"].ToString();
                String yearsExperience = reader["yearsExperience"].ToString();

                Console.WriteLine($"{firstName} \t {lastName} \t {phoneNumber} \t {emailAddress} \t {yearsExperience}");
            }

            connection.Close();
        }

        public void updateBiography(String newBio)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            if (!isCurrentUserInstructor)
            {
                Console.WriteLine("This function is not available to students.");
                return;
            }

            String queryText = "update instructor\n" +
                                $"set biography = '{newBio}'\n" +
                                $"where emailAddress like '{currentUser}';";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void displayInstructorBiography(String firstName, String lastName)
        {
            if (currentUser == null)
            {
                Console.WriteLine("Must be logged in.");
                return;
            }

            String queryText = "select biography\n" +
                                "from instructor\n" +
                                $"where firstname like '{firstName}' AND lastname like '{lastName}'; ";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;

            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            Console.WriteLine(reader["biography"].ToString());

            connection.Close();
        }

        public void userLogout()
        {
            currentUser = null;
            isCurrentUserInstructor = false;
        }

        public void completeClass(String classTitle, String classStartDate, int classStartTime, String studentFirstName, String studentLastName)
        {
            if (!isCurrentUserInstructor)
            {
                Console.WriteLine("This function is not available to students.");
                return;
            }

            String queryText = "update enrolled\n" +
                                "set statusid = 2\n" +
                                "where classinstanceid = (select classinstance.id\n" +
                                                        "from classinstance\n" +
                                                            "join class on (class.id = classinstance.id)\n" +
                                                        $"where title like '{classTitle}' AND startDate = date('{classStartDate}') AND startTime = {classStartTime})\n" +
                                    "AND studentid = (select student.id\n" +
                                                    "from student\n" +
                                                    $"where firstName like '{studentFirstName}' AND lastName like '{studentLastName}');";

            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = queryText;
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}
