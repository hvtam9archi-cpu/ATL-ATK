using System;
using System.Collections.Generic;
using System.Linq;
using ATL_ATK.Helpers;
using ATL_ATK.Models;

namespace ATL_ATK.Services
{
    /// <summary>
    /// Service sắp xếp danh sách Block theo tọa độ hoặc theo cột.
    /// Tương đương ND:ATK_sortXY và các sort trong LISP.
    /// </summary>
    public static class SortingService
    {
        /// <summary>Các hướng sắp xếp (tương đương 3DUY-ATK-SORT-POS-LIST)</summary>
        public static readonly string[] SortDirectionLabels = new[]
        {
            "Trái -> Phải",   // 0
            "Phải -> Trái",   // 1
            "Trên -> Dưới",   // 2
            "Dưới -> Trên"    // 3
        };

        /// <summary>
        /// Sắp xếp danh sách Block theo tọa độ XY.
        /// Tương đương ND:ATK_sortXY trong LISP.
        /// 
        /// direction1: hướng sắp xếp chính (0-3)
        /// direction2: hướng sắp xếp phụ (0-3)
        /// fuzz: dung sai nhóm (blocks có tọa độ trong phạm vi fuzz coi là cùng hàng/cột)
        /// </summary>
        public static List<BlockData> SortByPosition(
            List<BlockData> blocks,
            int direction1,
            int direction2,
            double fuzz)
        {
            if (blocks == null || blocks.Count <= 1)
                return blocks;

            // Nhóm blocks theo tọa độ chính (dùng fuzz)
            var groups = GroupByCoordinate(blocks, direction1, fuzz);

            // Sắp xếp các nhóm theo hướng chính
            groups = SortGroups(groups, direction1);

            // Trong mỗi nhóm, sắp xếp theo hướng phụ
            var result = new List<BlockData>();
            foreach (var group in groups)
            {
                var sortedGroup = SortWithinGroup(group, direction2);
                result.AddRange(sortedGroup);
            }

            return result;
        }

        /// <summary>
        /// Sắp xếp danh sách các hàng dữ liệu (string[][]) theo cột chỉ định.
        /// Tương đương sort theo 3DUY-ATK-SORT-COL trong LISP.
        /// </summary>
        public static List<List<string>> SortByColumnValue(
            List<List<string>> rows,
            int columnIndex)
        {
            var comparer = new AlphanumComparer();

            return rows.OrderBy(row =>
            {
                if (columnIndex >= 0 && columnIndex < row.Count)
                    return row[columnIndex];
                return "";
            }, comparer).ToList();
        }

        /// <summary>
        /// Sắp xếp danh sách BlockData theo tên Block (alphanumeric).
        /// </summary>
        public static List<BlockData> SortByBlockName(List<BlockData> blocks)
        {
            var comparer = new AlphanumComparer();
            return blocks.OrderBy(b => b.BlockName, comparer).ToList();
        }

        // -------------------------------------------------------
        //  PRIVATE HELPERS
        // -------------------------------------------------------

        /// <summary>
        /// Lấy tọa độ chính dùng để nhóm/sắp xếp theo direction.
        /// 0,1 (trái-phải) → dùng X để nhóm, Y để sắp xếp phụ
        /// 2,3 (trên-dưới) → dùng Y để nhóm, X để sắp xếp phụ
        /// </summary>
        private static double GetPrimaryCoord(BlockData block, int direction)
        {
            // Hướng trái-phải → nhóm theo Y (cùng hàng)
            // Hướng trên-dưới → nhóm theo X (cùng cột)
            if (direction == 0 || direction == 1)
                return block.Y;
            else
                return block.X;
        }

        private static double GetSecondaryCoord(BlockData block, int direction)
        {
            if (direction == 0 || direction == 1)
                return block.X;
            else
                return block.Y;
        }

        private static List<List<BlockData>> GroupByCoordinate(List<BlockData> blocks, int direction, double fuzz)
        {
            // Sắp xếp theo tọa độ chính trước
            var sorted = blocks.OrderBy(b => GetPrimaryCoord(b, direction)).ToList();

            var groups = new List<List<BlockData>>();
            var currentGroup = new List<BlockData> { sorted[0] };
            double currentCoord = GetPrimaryCoord(sorted[0], direction);

            for (int i = 1; i < sorted.Count; i++)
            {
                double coord = GetPrimaryCoord(sorted[i], direction);

                if (Math.Abs(coord - currentCoord) <= fuzz)
                {
                    currentGroup.Add(sorted[i]);
                }
                else
                {
                    groups.Add(currentGroup);
                    currentGroup = new List<BlockData> { sorted[i] };
                    currentCoord = coord;
                }
            }

            groups.Add(currentGroup);
            return groups;
        }

        private static List<List<BlockData>> SortGroups(List<List<BlockData>> groups, int direction)
        {
            switch (direction)
            {
                case 0: // Trái -> Phải: nhóm theo Y giảm dần (trên trước)
                case 1: // Phải -> Trái: nhóm theo Y giảm dần
                    return groups.OrderByDescending(g => g.Average(b => b.Y)).ToList();

                case 2: // Trên -> Dưới: nhóm theo X tăng dần
                    return groups.OrderBy(g => g.Average(b => b.X)).ToList();

                case 3: // Dưới -> Trên: nhóm theo X giảm dần
                    return groups.OrderByDescending(g => g.Average(b => b.X)).ToList();

                default:
                    return groups;
            }
        }

        private static List<BlockData> SortWithinGroup(List<BlockData> group, int direction)
        {
            switch (direction)
            {
                case 0: // Trái -> Phải
                    return group.OrderBy(b => b.X).ToList();
                case 1: // Phải -> Trái
                    return group.OrderByDescending(b => b.X).ToList();
                case 2: // Trên -> Dưới
                    return group.OrderByDescending(b => b.Y).ToList();
                case 3: // Dưới -> Trên
                    return group.OrderBy(b => b.Y).ToList();
                default:
                    return group;
            }
        }
    }
}
