using SFML.Window;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Snake
{
    class AI
    {
        static List<Vector2i> BFS(List<Vector2i> snakePositions, Vector2i applePosition)
        {
            Vector2i goal = applePosition;
            Vector2i start = snakePositions[0];
            Queue<Vector2i> q = new Queue<Vector2i>();
            bool[,] visited = new bool[Game.gridW, Game.gridH];
            Dictionary<Vector2i, Vector2i> parent = new Dictionary<Vector2i, Vector2i>();

            Vector2i[] directions =
            {
                new Vector2i(-1, 0),
                new Vector2i(0, 1),
                new Vector2i(1, 0),
                new Vector2i(0, -1)
            };

            //Kuyruk hariç yılanın vücudunun kapladığı her kareyi dolu olarak ata
            bool[,] blocked = new bool[Game.gridW, Game.gridH];
            for (int i = 0; i < snakePositions.Count - 1; i++)
            {
                blocked[snakePositions[i].X, snakePositions[i].Y] = true;
            }

            q.Enqueue(start);
            visited[start.X, start.Y] = true;

            while (q.Count > 0)
            {
                Vector2i current = q.Dequeue();

                if (current.Equals(goal)) return ReconstructPath(parent, start, goal);

                foreach (Vector2i direction in directions)
                {
                    int nx = current.X + direction.X;
                    int ny = current.Y + direction.Y;

                    if (nx < 0) nx = Game.gridW - 1;
                    if (nx >= Game.gridW) nx = 0;
                    if (ny < 0) ny = Game.gridH - 1;
                    if (ny >= Game.gridH) ny = 0;

                    if (!visited[nx, ny] && !blocked[nx, ny])
                    {
                        visited[nx, ny] = true;
                        parent[new Vector2i(nx, ny)] = current;
                        q.Enqueue(new Vector2i(nx, ny));
                    }
                }
            }

            return null;
        }

        static List<Vector2i> ReconstructPath(Dictionary<Vector2i, Vector2i> parent, Vector2i start, Vector2i goal)
        {
            List<Vector2i> path = new List<Vector2i>();
            Vector2i current = goal;

            while (!current.Equals(start))
            {
                path.Add(current);
                current = parent[current];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }

        static bool checkPathForCollision(List<Vector2i> path, List<Vector2i> snakePositions, Vector2i applePosition)
        {
            if (path == null || path.Count == 0)
            {
                MessageBox.Show("no path");
                return true;
            }

            //path[0] == head
            for (int i = 1; i < path.Count; i++)
            {
                //path'de hareket ettikçe bir sonraki sefer yılanın kafasının olacağı yer
                Vector2i nHead = path[i];

                //kuyruk hariç collision için kontrol et
                for (int j = 0; j < snakePositions.Count - 1; j++)
                {
                    if (snakePositions[j].Equals(nHead))
                        return true;
                }

                for (int s = snakePositions.Count - 1; s > 0; s--)
                    snakePositions[s] = snakePositions[s - 1];

                snakePositions[0] = nHead;

                if (nHead.Equals(applePosition))
                {
                    snakePositions.Add(snakePositions[snakePositions.Count - 1]);
                }

            }

            return false;
        }

        public static List<Vector2i> SafePathToApple(List<Vector2i> snakePositions, Vector2i applePosition)
        {
            List<Vector2i> path = BFS(snakePositions, applePosition);
            if (path != null && !checkPathForCollision(path, snakePositions, applePosition))
                return path;

            else return null;
        }
    }
}