
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GersonFrame.Tool
{
    public enum ReflectToughType
    {
        Left,
        Right,
        Up,
        Down,
        None,
    }
    public static class MathTool
    {
        public static T GetRandomValueFrom<T>(params T[] valuses)
        {
            int index = UnityEngine.Random.Range(0, valuses.Length);
            return valuses[index];
        }

        /// <summary>
        /// 获取格式化数据
        /// </summary>
        /// <param name="formatvalue"></param>
        /// <returns></returns>
        public static string GetFormartValue(float formatvalue, int showcount = 4)
        {
            if (formatvalue >= 10000)
            {
                return (int)((formatvalue * 1f / 1000) * Mathf.Pow(10, showcount - (formatvalue / 1000).ToString().Length)) / Mathf.Pow(10, showcount - (formatvalue / 1000).ToString().Length) + "K";
            }
            else return Mathf.FloorToInt(formatvalue).ToString();
        }

        /// <summary>
        /// 获取XZ轴距离
        /// </summary>
        /// <returns></returns>
        public static float GetXZDistance(Vector3 pos1, Vector3 pos2)
        {
            float disx = pos1.x - pos2.x;
            float disz = pos1.z - pos2.z;
            float dis = Mathf.Sqrt(disx * disx + disz * disz);
            return dis;
        }

        /// <summary>
        /// 获取UUiD
        /// </summary>
        /// <returns></returns>
        public static string GenUUID()
        {
            string uuid = System.Guid.NewGuid().ToString("X");
            /*    System.Guid.NewGuid().ToString(); 9af7f46a-ea52-4aa3-b8c3-9fd484c2af12
               string uuidN = System.Guid.NewGuid().ToString("N"); // e0a953c3ee6040eaa9fae2b667060e09 
                      string uuidD = System.Guid.NewGuid().ToString("D"); // 9af7f46a-ea52-4aa3-b8c3-9fd484c2af12
                      string uuidB = System.Guid.NewGuid().ToString("B"); // {734fd453-a4f8-4c5d-9c98-3fe2d7079760}
                      string uuidP = System.Guid.NewGuid().ToString("P"); //  (ade24d16-db0f-40af-8794-1e08e2040df3)
                      string uuidX = System.Guid.NewGuid().ToString("X"); // {0x3fa412e3,0x8356,0x428f,{0xaa,0x34,0xb7,0x40,0xda,0xaf,0x45,0x6f}}*/
            return uuid;
        }


        /// <summary>
        /// 时间戳(秒)
        /// </summary>
        public static long TimeStamp
        {
            get
            {

                System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                long timeStamp = (System.DateTime.Now - startTime).Ticks / 10000;
                return timeStamp;
            }
        }

        /// <summary>
        /// 获取反射向量 限定在二维平面内
        /// </summary>
        public static Vector3 GetReflectDir(Vector3 orildir, Vector3 planeDir)
        {
            Vector3 normal = GetNormalDir(planeDir);
            Vector3 reflectdir = Vector3.Reflect(orildir, normal);
            return reflectdir;
        }

        /// <summary>
        /// 获取向量的法向量
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 GetNormalDir(Vector3 vector)
        {
            if (vector.z == 0)
                return new Vector3(0, 0, 1);
            else
                return new Vector3(-vector.z / vector.x, 0, 1).normalized;
        }


        /// <summary>
        /// 围绕某点旋转指定角度
        /// </summary>
        /// <param name="position">自身坐标</param>
        /// <param name="center">旋转中心</param>
        /// <param name="axis">围绕旋转轴</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(angle, axis) * (position - center) + center;
        }


        public static Quaternion CalQuaternion(Vector3 dir)
        {
            Quaternion cal = new Quaternion();
            Vector3 euler = Quaternion.LookRotation(dir).eulerAngles;

            //欧拉角Y: cosY = z/sqrt(x^2+z^2)
            float CosY = dir.z / Mathf.Sqrt(dir.x * dir.x + dir.z * dir.z);
            float CosYDiv2 = Mathf.Sqrt((CosY + 1) / 2);
            if (dir.x < 0) CosYDiv2 = -CosYDiv2;

            float SinYDiv2 = Mathf.Sqrt((1 - CosY) / 2);

            //欧拉角X: cosX = sqrt((x^2+z^2)/(x^2+y^2+z^2)
            float CosX = Mathf.Sqrt((dir.x * dir.x + dir.z * dir.z) / (dir.x * dir.x + dir.y * dir.y + dir.z * dir.z));
            if (dir.z < 0) CosX = -CosX;
            float CosXDiv2 = Mathf.Sqrt((CosX + 1) / 2);
            if (dir.y > 0) CosXDiv2 = -CosXDiv2;
            float SinXDiv2 = Mathf.Sqrt((1 - CosX) / 2);

            //四元数w = cos(x/2)cos(y/2)
            cal.w = CosXDiv2 * CosYDiv2;
            //四元数x = sin(x/2)cos(y/2)
            cal.x = SinXDiv2 * CosYDiv2;
            //四元数y = cos(x/2)sin(y/2)
            cal.y = CosXDiv2 * SinYDiv2;
            //四元数z = sin(x/2)sin(y/2)
            cal.z = -SinXDiv2 * SinYDiv2;


            return cal;
        }



        /// <summary>
        /// 获取随机不重复数量 左闭右开
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="getcount"></param>
        /// <returns></returns>
        public static List<int> GetRandomValueNoRepeat(int min, int max, int getcount)
        {
            List<int> values = new List<int>();
            List<int> returnvalues = new List<int>();
            for (int i = min; i < max; i++)
            {
                values.Add(i);
            }

            for (int i = 0; i < getcount; i++)
            {
                if (values.Count <= 0)
                {
                    MyDebuger.LogError("随机数量不足 ");
                    break;
                }
                int valueindex = Random.Range(0, values.Count);
                returnvalues.Add(values[valueindex]);
                values.RemoveAt(valueindex);
            }
            return returnvalues;
        }


        /// <summary>
        /// 获取圆上的点
        /// </summary>
        /// <param name="oril"></param>
        /// <param name="raudio"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Vector3[] GetCyclePoses(Vector3 oril, float raudio, int count, bool worldforward = true, bool random = false)
        {
            if (count <= 0)
            {
                Debug.LogError("GetCyclePoses count less 0");

                Vector3[] pos = new Vector3[1];
                pos[0] = oril;
                return pos;
            }

            float randomAngle = 0;
            if (random) randomAngle = Random.Range(0, 360);

            Vector3[] poss = new Vector3[count];
            float angleRad = Mathf.Deg2Rad * 360;
            float angleCur = angleRad + randomAngle;
            float angledelta = angleRad / count;

            for (int i = 0; i < count; i++)
            {
                float cosA = Mathf.Cos(angleCur);
                float sinA = Mathf.Sin(angleCur);
                if (worldforward)
                    poss[i] = oril + new Vector3(raudio * sinA, 0, raudio * cosA);
                else
                    poss[i] = oril + new Vector3(-raudio * sinA, 0, -raudio * cosA);
                angleCur -= angledelta;
            }
            return poss;
        }


        /// <summary>
        /// 获取固定间隔角度的点
        /// </summary>
        /// <param name="orilpos"></param>
        /// <param name="stratdir"></param>
        /// <param name="angleinternal"></param>
        /// <param name="radio"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Vector3[] GetRangeRadPoint(Vector3 orilpos, Vector3 stratdir, float angleinternal, float radio, int count)
        {
            float maxangle = angleinternal * count;
            Vector3[] poses = new Vector3[count];
            float angle = maxangle / 2;
            if (count <= 1)
            {
                poses[0] = orilpos + stratdir * radio;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    float temoangle = -angle + i * maxangle / (count - 1);
                    poses[i] = orilpos + Quaternion.AngleAxis(temoangle, Vector3.up) * stratdir * radio;
                }
            }
            return poses;

        }

        /// <summary>
        /// 计算射线和面的交点 
        /// 会有一定误差 ， 浮点数计算没有办法
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="normal">面的法线</param>
        /// <param name="Point">面上的一点</param>
        /// <param name="ret">交点</param>
        /// <returns>线和面是否相交</returns>
        public static bool IntersectionOfRayAndFace(Ray ray, Vector3 normal, Vector3 Point, out Vector3 ret)
        {
            float dirvalue = Vector3.Dot(ray.direction, normal);
            if (dirvalue >= 0)
            {
                //如果平面法线和射线垂直 则不会相交
                ret = Vector3.zero;
                return false;
            }
            Vector3 Forward = normal;
            Vector3 Offset = Point - ray.origin; //获取线的方向
            float DistanceZ = Vector3.Angle(Forward, Offset); //计算夹角
            DistanceZ = Mathf.Cos(DistanceZ / 180f * Mathf.PI) * Offset.magnitude; //算点到面的距离
            DistanceZ /= Mathf.Cos(Vector3.Angle(ray.direction, Forward) / 180f * Mathf.PI); //算点沿射线到面的距离
            ret = ray.origin + ray.direction * DistanceZ; //算得射线和面的交点
            return true;
        }


        /// <summary>
        /// 阿拉伯数字转换成中文数字
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string NumToChinese(string x)
        {
            string[] pArrayNum = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            //为数字位数建立一个位数组
            string[] pArrayDigit = { "", "十", "百", "千" };
            //为数字单位建立一个单位数组
            string[] pArrayUnits = { "", "万", "亿", "万亿" };
            var pStrReturnValue = ""; //返回值
            var finger = 0; //字符位置指针
            var pIntM = x.Length % 4; //取模
            int pIntK;
            if (pIntM > 0)
                pIntK = x.Length / 4 + 1;
            else
                pIntK = x.Length / 4;
            //外层循环,四位一组,每组最后加上单位: ",万亿,",",亿,",",万,"
            for (var i = pIntK; i > 0; i--)
            {
                var pIntL = 4;
                if (i == pIntK && pIntM != 0)
                    pIntL = pIntM;
                //得到一组四位数
                var four = x.Substring(finger, pIntL);
                var P_int_l = four.Length;
                //内层循环在该组中的每一位数上循环
                for (int j = 0; j < P_int_l; j++)
                {
                    //处理组中的每一位数加上所在的位
                    int n = System.Convert.ToInt32(four.Substring(j, 1));
                    if (n == 0)
                    {
                        if (j < P_int_l - 1 && System.Convert.ToInt32(four.Substring(j + 1, 1)) > 0 && !pStrReturnValue.EndsWith(pArrayNum[n]))
                            pStrReturnValue += pArrayNum[n];
                    }
                    else
                    {
                        if (!(n == 1 && (pStrReturnValue.EndsWith(pArrayNum[0]) | pStrReturnValue.Length == 0) && j == P_int_l - 2))
                            pStrReturnValue += pArrayNum[n];
                        pStrReturnValue += pArrayDigit[P_int_l - j - 1];
                    }
                }
                finger += pIntL;
                //每组最后加上一个单位:",万,",",亿," 等
                if (i < pIntK) //如果不是最高位的一组
                {
                    if (System.Convert.ToInt32(four) != 0)
                        //如果所有4位不全是0则加上单位",万,",",亿,"等
                        pStrReturnValue += pArrayUnits[i - 1];
                }
                else
                {
                    //处理最高位的一组,最后必须加上单位
                    pStrReturnValue += pArrayUnits[i - 1];
                }
            }
            return pStrReturnValue;
        }

        /// <summary>
        /// 时间换算分秒
        /// </summary>
        /// <returns>The time.</returns>
        /// <param name="inputTime">输入的时间</param>
        public static string UpdateTime(float inputTime)
        {
            string temp;
            int minute, seconds;
            minute = (int)(inputTime / 60);

            seconds = (int)(inputTime % 60);
            //优化版,利用三目运算符进行取值,这样更好的实现倒计时
            string minTemp = (minute < 10) ? "0" + minute : minute.ToString();
            string secTemp = (seconds < 10) ? "0" + seconds : seconds.ToString();
            temp = minTemp + ":" + secTemp;

            return temp;
        }


        /// <summary>
        /// 将权重总和切成N个桶，N就是weights的数量，桶的大小就是平均权重。
        ///从weights中得到一个小于平均权重的列表，和一个大于等于平均权重的列表。
        ///取出一个小权重放入桶中，桶还有一点空间用来放一个大权重的一部分。
        ///一直重复这个过程，直到桶都填完，最终得到aliases这个数据结构。
        ///的索引是小权重的索引，aliases的每个元素由两个值组成：第一个是小权重占的比例，第二个是大权重的索引
        ///获取索引参考方法 
        /// static System.Random random = new System.Random();
        ///public static int GetRandomIndex((float, int)[] PrepareAdRewardWeight)
        /// {
        ///    var randomNum = random.NextDouble() * PrepareAdRewardWeight.Length;
        /// int intRan = (int)System.Math.Floor(randomNum);
        ///  var p = PrepareAdRewardWeight[intRan];
        ///  if (p.Item1 > randomNum - intRan)
        ///     return intRan;
        ///  else
        ///        return p.Item2;
        ///  }
        /// 
        /// </summary>
        /// <param name="weightList"></param>
        public static (float, int)[] InitAdRewardWeight(int[] weightList)
        {
            //权重总和
            var total = weightList.Sum();
            //权重数组长度
            int length = weightList.Length;
            //设置权重平均值
            var avg = 1f * total / length;

            //小于平均值的权重值和权重下标
            List<(float, int)> smallAvg = new List<(float, int)>();
            //大于平均值的权重值和权重下标
            List<(float, int)> bigAvg = new List<(float, int)>();

            //按权重划分大小桶
            for (int i = 0; i < weightList.Length; i++)
            {
                (weightList[i] > avg ? bigAvg : smallAvg).Add((weightList[i], i));
            }

            //PrepareAdRewardWeight的索引是小权重的索引，PrepareAdRewardWeight的每个元素由两个值组成：第一个是小权重占的比例，第二个是大权重的索引
            (float, int)[] PrepareAdRewardWeight = new (float, int)[weightList.Length];

            //对桶进行拆分 先将小桶塞入平均桶 
            for (int i = 0; i < weightList.Length; i++)
            {
                if (smallAvg.Count > 0)
                {
                    if (bigAvg.Count > 0)
                    {
                        PrepareAdRewardWeight[smallAvg[0].Item2] = (smallAvg[0].Item1 / avg, bigAvg[0].Item2);
                        //大权重变成大权重减去填补平均权重后的 值和索引
                        bigAvg[0] = (bigAvg[0].Item1 - avg + smallAvg[0].Item1, bigAvg[0].Item2);
                        //减去平均权重的值后 判断大权重和平均权重的大小 小于平均权重则添加到小权重 从大权重中删除
                        if (avg - bigAvg[0].Item1 > 0.0000001f)
                        {
                            smallAvg.Add(bigAvg[0]);
                            bigAvg.RemoveAt(0);
                        }
                    }
                    else
                    {
                        PrepareAdRewardWeight[smallAvg[0].Item2] = (smallAvg[0].Item1 / avg, smallAvg[0].Item2);
                    }
                    smallAvg.RemoveAt(0);
                }
                else
                {
                    PrepareAdRewardWeight[bigAvg[0].Item2] = (bigAvg[0].Item1 / avg, bigAvg[0].Item2);
                    bigAvg.RemoveAt(0);
                }
            }
            return PrepareAdRewardWeight;

        }



 
        /// <summary>
        /// 获取水平 左右边界的反射向量
        /// </summary>
        /// <param name="leftborder"></param>
        /// <param name="rightborder"></param>
        /// <param name="posx"></param>
        /// <param name="movedir"></param>
        /// <param name="toughType"></param>
        /// <returns></returns>
        public static Vector3 HorizontalReflectDir(float leftborder, float rightborder, float posx, Vector3 movedir, ref ReflectToughType toughType)
        {
            Vector3 normaldir = Vector3.zero;
            if (posx > rightborder)
            {
                if (toughType != ReflectToughType.Right)
                {
                    normaldir.x = -1;
                    toughType = ReflectToughType.Right;
                    if (Vector3.Dot(movedir, normaldir) > 0)
                        return movedir;
                    movedir = Vector3.Reflect(movedir, normaldir).normalized;
                }
            }
            if (posx < leftborder)
            {
                if (toughType != ReflectToughType.Left)
                {
                    toughType = ReflectToughType.Left;
                    normaldir.x = 1;
                    movedir = Vector3.Reflect(movedir, normaldir).normalized;
                }
            }
            movedir.y = 0;
            return movedir;
        }

        /// <summary>
        /// 获取水平 上下边界的反射向量
        /// </summary>
        /// <param name="leftborder"></param>
        /// <param name="rightborder"></param>
        /// <param name="posx"></param>
        /// <param name="movedir"></param>
        /// <param name="toughType"></param>
        /// <returns></returns>
        public static Vector3 VerticalReflectDir(float upborder, float downborder, float posz, Vector3 movedir, ref ReflectToughType toughType)
        {
            Vector3 normaldir = Vector3.zero;
            if (posz > upborder)
            {
                if (toughType != ReflectToughType.Up)
                {
                    normaldir.z = -1;
                    toughType = ReflectToughType.Up;
                    if (Vector3.Dot(movedir, normaldir) > 0)
                        return movedir;
                    movedir = Vector3.Reflect(movedir, normaldir).normalized;
                }
            }
            if (posz < downborder)
            {
                if (toughType != ReflectToughType.Down)
                {
                    toughType = ReflectToughType.Down;
                    normaldir.z = 1;
                    movedir = Vector3.Reflect(movedir, normaldir).normalized;
                }
            }
            movedir.y = 0;
            return movedir;
        }

        public static void CalculateTime(int index, ref float timer)
        {
            MyDebuger.LogWarning($"calculate count {index} time { Time.realtimeSinceStartup - timer} ");
            timer = Time.realtimeSinceStartup;
        }



    }




}

