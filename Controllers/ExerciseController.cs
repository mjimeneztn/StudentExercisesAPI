using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesAPI.Models;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private string _connectionString;

        public ExerciseController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }
        // GET: api/Exercise
        [HttpGet]
        public IEnumerable<Exercise> Get()
        {


            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, ExerciseName, ExerciseLanguage FROM Exercise";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Exercise> exercises = new List<Exercise>();
                    while (reader.Read())
                    {
                        Exercise newExercise = new Exercise()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            exerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            exerciseLanguage = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                        };
                        exercises.Add(newExercise);
                    }

                    reader.Close();

                    return exercises;
                }
            }
        }


        // GET: api/Exercise/5
        
        [HttpGet("{id}", Name = "GetExercise")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, ExerciseName, ExerciseLanguage
                                          FROM Exercise
                                         WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise exercise = null;

                    if (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            exerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            exerciseLanguage = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                        };
                    }
                    reader.Close();

                    return Ok(exercise);
                }
            }
        }


        // POST: api/Exercise
        [HttpPost]
        public void Post([FromBody] Exercise newExercise)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercise (exerciseName, exerciseLanguage)
                                        VALUES (@ExerciseName, @ExerciseLanguage)";
                    cmd.Parameters.Add(new SqlParameter("@ExerciseName", newExercise.exerciseName));
                    cmd.Parameters.Add(new SqlParameter("@ExerciseLanguage", newExercise.exerciseLanguage));

                    cmd.ExecuteNonQuery();


                }
            }
        }


        // PUT: api/Exercise/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Exercise
                                            SET ExerciseName = @ExerciseName,
                                                ExerciseLanguage = @ExerciseLanguage
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@ExerciseName", exercise.exerciseName));
                        cmd.Parameters.Add(new SqlParameter("@ExerciseLanguage", exercise.exerciseLanguage));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ExerciseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Exercise WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ExerciseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ExerciseExists(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, ExerciseName, ExerciseLanguage
                        FROM Exercise
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}

