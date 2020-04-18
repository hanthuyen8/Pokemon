using System.Collections.Generic;

namespace Game
{
    public static class Hints
    {
        public static List<Block> FindAHint(Block[,] arrays)
        {
            bool found = false;
            List<Block> hint = new List<Block>();
            var copied = arrays.Clone() as Block[,];

            for (int col = 0; col < copied.GetLength(0); col++)
            {
                for (int row = 0; row < copied.GetLength(1); row++)
                {
                    var block = copied[col, row];
                    if (block == null)
                    {
                        continue;
                    }
                    else
                    {
                        copied[col, row] = null;
                        hint.Add(block);
                        FindFromThisBlock(block);
                        if (hint.Count <= GameSettings.MIN_AMOUNT_OF_BLOCKS_TO_COMBINE)
                            hint.Clear();
                        else
                            found = true;
                    }

                    if (found)
                        break;
                }

                if (found)
                    break;
            }

            return hint;

            void FindFromThisBlock(Block block)
            {
                for (int i = 0; i < block.Neighbours.Count; i++)
                {
                    var neighbour = block.Neighbours[i];
                    var neighbourBlock = copied[neighbour.AtColumn, neighbour.AtRow];
                    if (neighbourBlock == null)
                        continue;

                    if(neighbourBlock.HasItem.Type == block.HasItem.Type)
                    {
                        // found 1
                        hint.Add(neighbourBlock);
                        copied[neighbour.AtColumn, neighbour.AtRow] = null;
                        FindFromThisBlock(neighbourBlock);
                    }
                }
            }
        }
    }

}