import java.sql.*;
public class DatabaseAPI {

    private static final String url = "jdbc:mysql//localhost:3306/CoolDudesDanceStudio";
    private static final String user = "root";
    private static final String password = "weAreSuperCool!";

    private static Connection con;
    private static Statement stmt;
    private static ResultSet rs;

    public static void main(String[] args) {
        System.out.println("This is a test");
        String query = "select count(*) from Student";
        try {
            con = DriverManager.getConnection(url, user, password);

            stmt = con.createStatement;

            rs = stmt.executeQuery(query);

            while (rs.next()) {
                int count = rs.getInt(1);
                System.out.println("Total number of students in the table: " + count);
            }
        } catch (SQLException sqlEx) {
            sqlEx.printStackTrace();
        } finally {
            try { con.close(); } catch(SQLException se) { /*can't do anything */ }
            try { stmt.close(); } catch(SQLException se) { /*can't do anything */ }
            try { rs.close(); } catch(SQLException se) { /*can't do anything */ }
        }
    }
}
