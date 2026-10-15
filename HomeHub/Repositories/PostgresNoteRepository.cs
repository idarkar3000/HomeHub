using Dapper;
using HomeHub.Models;
using Npgsql;

namespace HomeHub.Repositories
{
    public class PostgresNoteRepository : INoteRepository
    {
        private readonly string _connectionString;

        public PostgresNoteRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"CREATE TABLE IF NOT EXISTS notes (
                        id SERIAL PRIMARY KEY,
                        content TEXT NOT NULL,
                        is_done BOOLEAN NOT NULL DEFAULT false,
                        kind TEXT NOT NULL DEFAULT 'note',
                        room_id INT NULL REFERENCES rooms(id) ON DELETE SET NULL,
                        created_at TIMESTAMP NOT NULL DEFAULT now()
                        );";

            connection.Execute(sql);
        }

        public int Save(Note note)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"INSERT INTO notes (content, is_done, kind, room_id) VALUES (@Content, @IsDone, @Kind, @RoomId) RETURNING id;";
            return connection.ExecuteScalar<int>(sql, note);
        }

        public List<Note> GetAll(string? kind)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, content AS Content, is_done AS IsDone, kind AS Kind, room_id AS RoomId, created_at AS CreatedAt
                   FROM notes
                   WHERE (@Kind IS NULL OR kind = @Kind)
                   ORDER BY is_done ASC, id DESC;";

            return connection.Query<Note>(sql, new { Kind = kind }).ToList();
        }

        public Note? GetById(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, content AS Content, is_done AS IsDone, kind AS Kind, room_id AS RoomId, created_at AS CreatedAt
                   FROM notes WHERE id = @Id;";
            return connection.QueryFirstOrDefault<Note>(sql, new { Id = id });
        }

        public void Update(Note note)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"UPDATE notes SET content = @Content, is_done = @IsDone, kind = @Kind, room_id = @RoomId WHERE id = @Id;";
            connection.Execute(sql, note);
        }

        public void Delete(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            connection.Execute("DELETE FROM notes WHERE id = @Id;", new { Id = id });
        }
    }
}
