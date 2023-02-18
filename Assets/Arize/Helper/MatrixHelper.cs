using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Ninsar.Helper
{
    public static class MatrixHelper
    {
        public static bool[,] JsonToMatrix(string json)
        {
            if (JsonConvert.DeserializeObject(json) is List<object> value)
            {
                var matrix = new bool[value.Count, ((List<object>)value[0]).Count];
            
                for (int indexX = 0; indexX < matrix.GetLength(0); indexX++)
                {
                    var list = (List<object>) value[indexX];
                
                    for (int indexY = 0; indexY < matrix.GetLength(1); indexY++)
                    {
                        matrix[indexX, indexY] = (bool) list[indexY];
                    }
                }
            
                return matrix;
            }

            return null;
        }
        
        public static string ToJson(this bool[,] matrix)
        {
            var objs = new List<List<bool>>();
            for (int indexX = 0; indexX < matrix.GetLength(0); indexX++)
            {
                var list = new List<bool>();
                for (int indexY = 0; indexY < matrix.GetLength(1); indexY++)
                {
                    list.Add(matrix[indexX, indexY]);
                }
                
                objs.Add(list);
            }

            return JsonConvert.SerializeObject(objs);
        }
        
        public static bool[,] Shrink(this bool[,] matrix)
        {
            var pos1 = new Vector2Int(matrix.GetLength(0), matrix.GetLength(1));
            var pos2 = new Vector2Int(-1, -1);

            for (int indexX = 0; indexX < matrix.GetLength(0); indexX++)
            {
                for (int indexY = 0; indexY < matrix.GetLength(1); indexY++)
                {
                    var hasIntersectionX = false;
                    var hasIntersectionY = false;
                    
                    for (int indexRight = 0; indexRight < matrix.GetLength(0); indexRight++)
                    {
                        if (matrix[indexRight, indexY])
                        {
                            hasIntersectionX = true;
                            
                            break;
                        }
                    }
                    
                    for (int indexUp = 0; indexUp < matrix.GetLength(0); indexUp++)
                    {
                        if (matrix[indexX, indexUp])
                        {
                            hasIntersectionY = true;
                            
                            break;
                        }
                    }

                    if (hasIntersectionX && hasIntersectionY && pos1.x >= indexX && pos1.y >= indexY)
                    {
                        pos1 = new Vector2Int(indexX, indexY);
                        break;
                    }
                }
            }
            
            if (pos1 == new Vector2Int(matrix.GetLength(0), matrix.GetLength(1)))
                return null;
            
            for (int indexX = matrix.GetLength(0) - 1; indexX >= 0; indexX--)
            {
                for (int indexY = matrix.GetLength(1) - 1; indexY >= 0; indexY--)
                {
                    var hasIntersectionX = false;
                    var hasIntersectionY = false;
                    
                    for (int indexRight = matrix.GetLength(0) - 1; indexRight >= 0; indexRight--)
                    {
                        if (matrix[indexRight, indexY])
                        {
                            hasIntersectionX = true;
                            
                            break;
                        }
                    }
                    
                    for (int indexUp = matrix.GetLength(0) - 1; indexUp >= 0; indexUp--)
                    {
                        if (matrix[indexX, indexUp])
                        {
                            hasIntersectionY = true;
                            
                            break;
                        }
                    }

                    if (hasIntersectionX && hasIntersectionY && pos2.x <= indexX && pos2.y <= indexY)
                    {
                        pos2 = new Vector2Int(indexX, indexY);
                        break;
                    }
                }
            }

            if (pos2 == Vector2Int.zero && pos1 != Vector2Int.zero)
                return null;

            var newMatrix = new bool[pos2.x - pos1.x + 1, pos2.y - pos1.y + 1];

            for (int i = pos1.x; i <= pos2.x; i++)
            {
                for (int j = pos1.y; j <= pos2.y; j++)
                {
                    newMatrix[i - pos1.x, j - pos1.y] = matrix[i, j];
                }
            }
            
            return newMatrix;
        }
    }
}