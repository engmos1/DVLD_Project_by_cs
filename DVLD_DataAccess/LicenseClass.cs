using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DVLD_DataAccess.clsCountryData;
using System.Net;
using System.Security.Policy;

namespace DVLD_DataAccess
{
    public class clsLicenseClassData
    {

        public static bool GetLicenseClassInfoByID(int LicenseClassID,
     ref string ClassName, ref string ClassDescription, ref byte MinimumAllowedAge,
     ref byte DefaultValidityLength, ref float ClassFees)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("sp_GetLicenseClassByID", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // The record was found
                            isFound = true;

                            ClassName = reader["ClassName"] as string ?? string.Empty;
                            ClassDescription = reader["ClassDescription"] as string ?? string.Empty;
                            MinimumAllowedAge = (byte)reader["MinimumAllowedAge"];
                            DefaultValidityLength = (byte)reader["DefaultValidityLength"];
                            ClassFees = Convert.ToSingle(reader["ClassFees"]);
                        }
                        // else record not found (isFound remains false)
                    }
                }
                catch (SqlException ex)
                {

                    // Logger.LogError(ex, "Error retrieving license class info");
                    isFound = false;
                }
            }

            return isFound;
        }

        public static bool GetLicenseClassInfoByClassName(string ClassName,
    ref int LicenseClassID, ref string ClassDescription, ref byte MinimumAllowedAge,
    ref byte DefaultValidityLength, ref float ClassFees)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("sp_GetLicenseClassByClassName", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ClassName", ClassName);

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Record found
                            isFound = true;
                            LicenseClassID = (int)reader["LicenseClassID"];
                            ClassDescription = reader["ClassDescription"] as string ?? string.Empty;
                            MinimumAllowedAge = (byte)reader["MinimumAllowedAge"];
                            DefaultValidityLength = (byte)reader["DefaultValidityLength"];
                            ClassFees = Convert.ToSingle(reader["ClassFees"]);
                        }

                    }
                }
                catch (SqlException ex)
                {
                    // Log error if needed
                    // Logger.LogError(ex, "Error retrieving license class by name");
                    isFound = false;
                }
            }

            return isFound;
        }

        public static DataTable GetAllLicenseClasses()
        {
            DataTable dataTable = new DataTable();


            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("sp_GetAllLicenseClasses", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dataTable.Load(reader);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Log the error properly in production
                    // Logger.LogError(ex, "Error retrieving all license classes");

                    // Return empty DataTable in case of error
                    return new DataTable();
                }
            }

            return dataTable;
        }

        public static int AddNewLicenseClass(string ClassName, string ClassDescription,
    byte MinimumAllowedAge, byte DefaultValidityLength, float ClassFees)
        {
            int LicenseClassID = -1;


            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("sp_AddNewLicenseClass", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@ClassName", ClassName);
                command.Parameters.AddWithValue("@ClassDescription", ClassDescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                command.Parameters.AddWithValue("@ClassFees", ClassFees);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        LicenseClassID = insertedID;
                    }
                }
                catch (SqlException ex)
                {
                    // Log the error properly in production
                    // Logger.LogError(ex, "Error adding new license class");

                    // You might want to throw the exception or handle it differently
                    // depending on your application requirements
                    return -1;
                }
            }

            return LicenseClassID;
        }

        public static bool UpdateLicenseClass(int LicenseClassID, string ClassName,
    string ClassDescription, byte MinimumAllowedAge,
    byte DefaultValidityLength, float ClassFees)
        {


            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("sp_UpdateLicenseClass", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters
                command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
                command.Parameters.AddWithValue("@ClassName", ClassName);
                command.Parameters.AddWithValue("@ClassDescription",
                    string.IsNullOrEmpty(ClassDescription) ? (object)DBNull.Value : ClassDescription);
                command.Parameters.AddWithValue("@MinimumAllowedAge", MinimumAllowedAge);
                command.Parameters.AddWithValue("@DefaultValidityLength", DefaultValidityLength);
                command.Parameters.AddWithValue("@ClassFees", ClassFees);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    // Log error (uncomment in production)
                    // Logger.LogError(ex, "Error updating license class");
                    return false;
                }
                catch (Exception ex)
                {
                    // Log unexpected errors
                    // Logger.LogError(ex, "Unexpected error updating license class");
                    return false;
                }
            }
        }

    }
}
