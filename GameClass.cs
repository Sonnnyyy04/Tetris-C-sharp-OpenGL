using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Graphics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;



namespace ConsoleApp3
{
    internal class GameClass : GameWindow
    {
        private int vertexBufferHandle;
        private int programShaderHandle;
        private int vertexArrayHandle;
        private System.Timers.Timer aTimer;
        const int CW = 48;
        const int FH = 24;
        const int FW = 16;
        const int LCD_TOTAL = 2;
        int curX = 5;
        int curY = FH - 4;
        const int LCD_W = 5;
        const int LCD_H = 7;
        const int SECTORS_PER_CIRCLE = 24;

        // colors palette
        float[,] allColors = new float[,]
        {
            {0f,0f,0f,1f },
            {1f,0f,0f,1f },
            {0f,1f,0f,1f },
            {0f,0f,1f,1f },
            {1f,1f,0f,1f },
            {1f,0f,1f,1f },
            {0f,1f,1f,1f },
            {0.25f,1f,0.75f,1f },
            {0.75f,0.75f,0f,1f },
            {0.5f,0.5f,1f,1f }
        };

        List<byte[,]> allDigits = new List<byte[,]>();


        List<byte[,]> lcdInfo = new List<byte[,]>();



        byte[,] cellColorsIndexes = new byte[FH, FW];

        int curFigureColorIndex = 1;
        int score = 0;

        List<int[,]> etalonFigures = new List<int[,]>();
        int[,] currentFigure = null;



        float[] vertices = new float[7 * sizeof(float) * (6 * FH * FW + LCD_TOTAL* LCD_H * LCD_W * SECTORS_PER_CIRCLE * 3)];



        public GameClass(string title = "Tetrix Game") :
            base(GameWindowSettings.Default,
                new NativeWindowSettings()
                {
                    Title = title,
                    Size = new Vector2i(400,800),
                    WindowBorder = WindowBorder.Fixed,
                    StartVisible = false,
                    StartFocused = true,
                    API = ContextAPI.OpenGL,
                    Profile = ContextProfile.Core,
                    APIVersion = new Version(3, 3)
                })

        {


            etalonFigures.Add(new int[,] { { 0, 0, 0, 0 }, { 0, 1, 1, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 } }); // square 2x2
            etalonFigures.Add(new int[,] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 1, 1, 1, 1 }, { 0, 0, 0, 0 } }); // line 1x4
            etalonFigures.Add(new int[,] { { 0, 0, 0, 0 }, { 0, 0, 1, 0 }, { 0, 1, 1, 1 }, { 0, 0, 0, 0 } }); // T-figure
            etalonFigures.Add(new int[,] { { 0, 0, 0, 0 }, { 0, 0, 1, 1 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 } }); // Z-figure
            etalonFigures.Add(new int[,] { { 0, 0, 0, 0 }, { 1, 1, 0, 0 }, { 0, 1, 1, 0 }, { 0, 0, 0, 0 } }); // Z-figure mirrored
            etalonFigures.Add(new int[,] { { 0, 0, 0, 0 }, { 0, 1, 1, 1 }, { 0, 1, 0, 0 }, { 0, 0, 0, 0 } }); // G-figure
            etalonFigures.Add(new int[,] { { 0, 0, 0, 0 }, { 0, 1, 1, 1 }, { 0, 0, 0, 1 }, { 0, 0, 0, 0 } }); // G-figure mirrored

            allDigits.Add(new byte[LCD_H, LCD_W]
            {
                {0,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {0,1,1,1,0}
            });

            allDigits.Add(new byte[LCD_H, LCD_W]
            {
                {0,0,1,0,0},
                {0,1,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,1,1,1,0}
            });

            allDigits.Add(new byte[LCD_H, LCD_W]
            {
                {0,1,1,1,0},
                {1,0,0,0,1},
                {0,0,0,1,0},
                {0,0,1,0,0},
                {0,1,0,0,0},
                {1,0,0,0,0},
                {1,1,1,1,1}
            });

            allDigits.Add(new byte[LCD_H, LCD_W]
            {
                {0,1,1,1,0},
                {1,0,0,0,1},
                {0,0,0,0,1},
                {0,1,1,1,0},
                {0,0,0,0,1},
                {1,0,0,0,1},
                {0,1,1,1,0}
            });

            allDigits.Add(new byte[LCD_H, LCD_W]
            {
                {0,0,0,1,1},
                {0,0,1,0,1},
                {0,1,0,0,1},
                {1,0,0,0,1},
                {1,1,1,1,1},
                {0,0,0,0,1},
                {0,0,0,0,1}
            });

            allDigits.Add(new byte[LCD_H, LCD_W]
            {
                {1,1,1,1,1},
                {1,0,0,0,0},
                {1,1,1,1,0},
                {0,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {0,1,1,1,0}
            });

            allDigits.Add(new byte[LCD_H, LCD_W]
            {
                {0,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,0},
                {1,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {0,1,1,1,0}
            });

            allDigits.Add(new byte[LCD_H, LCD_W]
            {
                {1,1,1,1,1},
                {1,0,0,0,1},
                {0,0,0,1,0},
                {0,0,1,0,0},
                {0,1,0,0,0},
                {0,1,0,0,0},
                {0,1,0,0,0}
            });

            allDigits.Add(new byte[LCD_H, LCD_W]
            {
                {0,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {0,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {0,1,1,1,0}
            });

            allDigits.Add(new byte[LCD_H, LCD_W]
            {
                {0,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {0,1,1,1,1},
                {0,0,0,0,1},
                {1,0,0,0,1},
                {0,1,1,1,0}
            });


            currentFigure = (int[,])etalonFigures[3].Clone();

            const float dd = 1.0f / 400;
            for (int yOff = 0; yOff < FH; ++yOff)
            {
                for (int xOff = 0; xOff < FW; ++xOff)
                {
                    
                    vertices[(yOff * FW + xOff) * 7 * 6 + 0 * 7 + 0] = -1 + 1.0f * xOff / FW * 2 + dd;
                    vertices[(yOff * FW + xOff) * 7 * 6 + 0 * 7 + 1] = -1 + 1.0f * yOff / FH * 2 + dd;

                    vertices[(yOff * FW + xOff) * 7 * 6 + 1 * 7 + 0] = -1 + 1.0f * (xOff + 1) / FW * 2 - dd;
                    vertices[(yOff * FW + xOff) * 7 * 6 + 1 * 7 + 1] = -1 + 1.0f * yOff / FH * 2 + dd;

                    vertices[(yOff * FW + xOff) * 7 * 6 + 2 * 7 + 0] = -1 + 1.0f * (xOff + 1) / FW * 2 - dd;
                    vertices[(yOff * FW + xOff) * 7 * 6 + 2 * 7 + 1] = -1 + 1.0f * (yOff + 1) / FH * 2 - dd;

                    vertices[(yOff * FW + xOff) * 7 * 6 + 3 * 7 + 0] = -1 + 1.0f * xOff / FW * 2 + dd;
                    vertices[(yOff * FW + xOff) * 7 * 6 + 3 * 7 + 1] = -1 + 1.0f * yOff / FH * 2 + dd;

                    vertices[(yOff * FW + xOff) * 7 * 6 + 4 * 7 + 0] = -1 + 1.0f * xOff / FW * 2 + dd;
                    vertices[(yOff * FW + xOff) * 7 * 6 + 4 * 7 + 1] = -1 + 1.0f * (yOff + 1) / FH * 2 - dd;

                    vertices[(yOff * FW + xOff) * 7 * 6 + 5 * 7 + 0] = -1 + 1.0f * (xOff + 1) / FW * 2 - dd;
                    vertices[(yOff * FW + xOff) * 7 * 6 + 5 * 7 + 1] = -1 + 1.0f * (yOff + 1) / FH * 2 - dd;
                }
            }

            int lcdOffset = FW * FH * 6 * 7;
            float lcd_rad = 0.015f;
            float lcd_step_x = 0.04f;
            float lcd_step_y = lcd_step_x * FW / FH;

            for (int digit = 0; digit < LCD_TOTAL; ++digit)            
            {
                int digitOff = digit * LCD_W * LCD_H * SECTORS_PER_CIRCLE * 3 * 7;

                for (int dy = 0; dy < LCD_H; dy++)
                {
                    for (int dx = 0; dx < LCD_W; ++dx)
                    {
                        for (int k = 0; k < SECTORS_PER_CIRCLE; ++k)
                        {
                            float cx = -1f + (1 + dx) * lcd_step_x+digit*(LCD_W+1)*lcd_step_x;
                            float cy = 1f - (1 + dy) * lcd_step_y;

                            vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 0 * 7 + 0] = cx;
                            vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 0 * 7 + 1] = cy;

                            vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 1 * 7 + 0] = cx + lcd_rad * (float)Math.Cos(k * 2 * Math.PI / SECTORS_PER_CIRCLE) * FH / FW;
                            vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 1 * 7 + 1] = cy + lcd_rad * (float)Math.Sin(k * 2 * Math.PI / SECTORS_PER_CIRCLE);

                            vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 2 * 7 + 0] = cx + lcd_rad * (float)Math.Cos((k + 1) * 2 * Math.PI / SECTORS_PER_CIRCLE) * FH / FW;
                            vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 2 * 7 + 1] = cy + lcd_rad * (float)Math.Sin((k + 1) * 2 * Math.PI / SECTORS_PER_CIRCLE);
                        }
                    }
                }
            }

            //ReColorCell(1, 1, 1);
            //ReColorCell(2, 2, 2);
            //ReColorCell(3, 3, 3);

            PutFigure(currentFigure, curFigureColorIndex);

            this.CenterWindow();

        }

        private bool CanMoveByDown()
        {
            int deltaX = 0;
            int deltaY = -1;

            Console.Write(String.Format("Move {0}:{1} ", deltaX, deltaY));
            for (int dy = 0; dy < currentFigure.GetLength(0); dy++)
            {
                for (int dx = 0; dx < currentFigure.GetLength(1); dx++)
                {
                    if (currentFigure[dy, dx] == 0) continue;
                    int newGlobalX = curX + dx + deltaX;
                    int newGlobalY = curY + dy + deltaY;

                    if (newGlobalX < 0 || newGlobalX >= FW || newGlobalY < 0 || newGlobalY >= FH || cellColorsIndexes[newGlobalY, newGlobalX] != 0)
                    {
                        Console.WriteLine("False " + newGlobalY.ToString() + ":" + newGlobalX.ToString());
                        return false;
                    }
                }
            }

            Console.WriteLine("True");
            return true;
        }


        private bool CanMoveByLeft()
        {
            int deltaX = -1;
            int deltaY = 0;

            Console.Write(String.Format("Move {0}:{1} ", deltaX, deltaY));
            for (int dy = 0; dy < currentFigure.GetLength(0); dy++)
            {
                for (int dx = 0; dx < currentFigure.GetLength(1); dx++)
                {
                    if (currentFigure[dy, dx] == 0) continue;
                    int newGlobalX = curX + dx + deltaX;
                    int newGlobalY = curY + dy + deltaY;

                    if (newGlobalX < 0 || newGlobalX >= FW || newGlobalY < 0 || newGlobalY >= FH || cellColorsIndexes[newGlobalY, newGlobalX] != 0)
                    {
                        Console.WriteLine("False");
                        return false;
                    }
                }
            }

            Console.WriteLine("True");
            return true;
        }

        private bool CanMoveByRight()
        {
            int deltaX = 1;
            int deltaY = 0;

            Console.Write(String.Format("Move {0}:{1} ", deltaX, deltaY));
            for (int dy = 0; dy < currentFigure.GetLength(0); dy++)
            {
                for (int dx = 0; dx < currentFigure.GetLength(1); dx++)
                {
                    if (currentFigure[dy, dx] == 0) continue;
                    int newGlobalX = curX + dx + deltaX;
                    int newGlobalY = curY + dy + deltaY;

                    if (newGlobalX < 0 || newGlobalX >= FW || newGlobalY < 0 || newGlobalY >= FH || cellColorsIndexes[newGlobalY, newGlobalX] != 0)
                    {
                        Console.WriteLine("False");
                        return false;
                    }
                }
            }

            Console.WriteLine("True");
            return true;
        }

        private bool CanRotate(int[,] a)
        {
            int[,] b = new int[a.GetLength(1), a.GetLength(0)];

            for (int k = 0; k < a.GetLength(0); ++k)
            {
                for (int j = 0; j < a.GetLength(1); ++j)
                {
                    int j2 = a.GetLength(0) - 1 - k;
                    int k2 = j;
                    b[k2, j2] = a[k, j];
                }
            }

            for (int dy = 0; dy < b.GetLength(0); ++dy)
            {
                for (int dx = 0; dx < b.GetLength(1); ++dx)
                {
                    int globalY = curY + dy;
                    int globalX = curX + dx;

                    if (b[dy, dx] == 0) continue;
                    if (globalY < 0 || globalY >= FH || globalX < 0 || globalX >= FW || cellColorsIndexes[globalY, globalX] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }



        private void PutFigure(int[,] cf, int cIndex)
        {
            for (int dy = 0; dy < cf.GetLength(0); ++dy)
            {
                for (int dx = 0; dx < cf.GetLength(1); ++dx)
                {
                    if (cf[dy, dx] == 1)
                    {
                        ReColorCell(curX + dx, curY + dy, cIndex);
                    }
                }
            }
        }

        private void ReColorCell(int xOff, int yOff, int cIndex)
        {
            for (int k = 0; k < 6; ++k)
            {
                for (int j = 0; j < 4; ++j)
                {
                    vertices[(yOff * FW + xOff) * 6 * 7 + k * 7 + 3 + j] = allColors[cIndex, j];
                }
            }
        }
        private Tuple<int, int> BadPositionEscapeVec()
        {

            for (int dy = 0; dy < currentFigure.GetLength(0); ++dy)
            {
                for (int dx = 0; dx < currentFigure.GetLength(1); ++dx)
                {

                    if (currentFigure[dy, dx] == 0) continue;

                    int globalX = curX + dx;
                    int globalY = curY + dy;


                    if (globalX < 0)
                    {
                        return new Tuple<int, int>(1, 0);
                    }
                    if (globalX >= FW)
                    {
                        return new Tuple<int, int>(-1, 0);
                    }
                    if (globalY < 0)
                    {
                        return new Tuple<int, int>(0, 1);
                    }
                    if (globalY >= FH)
                    {
                        return new Tuple<int, int>(0, -1);
                    }
                    if (cellColorsIndexes[globalY, globalX] != 0)
                    {
                        return new Tuple<int, int>(0, 1);
                    }
                }
            }
            return null;
        }

        void FindAndDestroyFullRow()
        {
            for (int row = 0; row < FH - 1; ++row)
            {
                int filledCnt = 0;
                for (int col = 0; col < FW; ++col)
                {
                    if (cellColorsIndexes[row, col] != 0)
                    {
                        ++filledCnt;
                    }                    
                }
                if (filledCnt == 0)
                {
                    Console.WriteLine("Empty row " + row);
                    return;
                }
                else if (filledCnt == FW)
                {
                    Console.WriteLine("Found full row #" + row.ToString());
                    for (int k = row; k < FH - 1; ++k)
                    {
                        for (int j = 0; j < FW; ++j)
                        {
                            cellColorsIndexes[k, j] = cellColorsIndexes[k + 1, j];
                        }
                    }

                    for (int k = row; k < FH; ++k)
                    {
                        for (int j = 0; j < FW; ++j)
                        {
                            ReColorCell(j, k, cellColorsIndexes[k, j]);
                        }
                    }
                    score++;
                    NewScore();
                    break;
                }
            }
        }

        private void NewScore()
        {
            lcdInfo.Clear();
            int tmp = score;

            while (lcdInfo.Count < LCD_TOTAL)
            {
                int digit = tmp % 10;
                lcdInfo.Insert(0, allDigits[digit]);
                tmp /= 10;
            }

            RefillLcd();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }
        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {

            if (e.Key == Keys.Escape)
            {
                Close();
            }
            else if (e.Key == Keys.Left)
            {
                if (CanMoveByLeft())
                {
                    PutFigure(currentFigure, 0);
                    curX--;
                }
                else
                {
                    return;
                }
            }
            else if (e.Key == Keys.Right)
            {
                if (CanMoveByRight())
                {
                    PutFigure(currentFigure, 0);
                    curX++;
                }
                else
                {
                    return;
                }
            }
            else if (e.Key == Keys.Down)
            {
                if (CanMoveByDown())
                {
                    PutFigure(currentFigure, 0);
                    curY--;
                }
                else
                {
                    return;
                }
            }
            else if (e.Key == Keys.Space)
            {

                if (!CanRotate(currentFigure))
                {
                    Console.Beep();
                    return;
                }

                PutFigure(currentFigure, 0);
                currentFigure = Rotate(currentFigure);
                Tuple<int, int> vec = null;
                while ((vec = BadPositionEscapeVec()) != null)
                {
                    curX += vec.Item1;
                    curY += vec.Item2;
                }
            }
            else if (e.Key == Keys.LeftShift)
            {
                if (aTimer.Enabled)
                {
                    aTimer.Enabled = false;
                }
                else
                {
                    aTimer.Enabled = true;
                }
            }


            PutFigure(currentFigure, curFigureColorIndex);
        }

        int[,] Rotate(int[,] a)
        {

            int[,] b = new int[a.GetLength(1), a.GetLength(0)];

            for (int k = 0; k < a.GetLength(0); ++k)
            {
                for (int j = 0; j < a.GetLength(1); ++j)
                {
                    int j2 = a.GetLength(0) - 1 - k;
                    int k2 = j;
                    b[k2, j2] = a[k, j];
                }
            }

            return b;
        }
        void TimerEvent(Object source, ElapsedEventArgs e)
        {
            var vec = BadPositionEscapeVec();
            if (CanMoveByDown())
            {
                PutFigure(currentFigure, 0);
                curY--;
                PutFigure(currentFigure, curFigureColorIndex);
            }
            else
            {
                FrozeAtPlace();
                FindAndDestroyFullRow();
                bool genSuccess = GenerateNewFigure();
                if (!genSuccess)
                {
                    Console.WriteLine("Game over!");
                    aTimer.Enabled = false;
                }
                else
                {                   
                }
            }

        }

        private void RefillLcd()
        {

            int lcdOffset = FW * FH * 6 * 7;


            float[] lcdColorYes = { 1f, 1f, 0f, 1f };
            float[] lcdColorNo = { 0f, 0f, 0f, 0f };

            for (int digit = 0; digit < LCD_TOTAL; ++digit)
            {
                int digitOff = digit * LCD_H * LCD_W * SECTORS_PER_CIRCLE * 3 * 7;
                for (int dy = 0; dy < LCD_H; dy++)
                {
                    for (int dx = 0; dx < LCD_W; ++dx)
                    {

                        for (int k = 0; k < SECTORS_PER_CIRCLE; ++k)
                        {

                            if (lcdInfo[digit][dy, dx] == 1)
                            {
                                for (int j = 0; j < 4; ++j)
                                {
                                    vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 0 * 7 + 3 + j] = lcdColorYes[j];
                                    vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 1 * 7 + 3 + j] = lcdColorNo[j];
                                    vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 2 * 7 + 3 + j] = lcdColorNo[j];
                                }
                            }
                            else
                            {
                                for (int j = 0; j < 4; ++j)
                                {
                                    vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 0 * 7 + 3 + j] = lcdColorNo[j];
                                    vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 1 * 7 + 3 + j] = lcdColorNo[j];
                                    vertices[lcdOffset + digitOff + ((dy * LCD_W + dx) * SECTORS_PER_CIRCLE + k) * 3 * 7 + 2 * 7 + 3 + j] = lcdColorNo[j];
                                }
                            }
                        }
                    }
                }
            }
        }


        void FrozeAtPlace()
        {
            for (int dy = 0; dy < currentFigure.GetLength(0); dy++)
            {
                for (int dx = 0; dx < currentFigure.GetLength(1); dx++)
                {
                    if (currentFigure[dy, dx] == 0) continue;


                    int newGlobalX = curX + dx;
                    int newGlobalY = curY + dy;
                    if (newGlobalX < 0 || newGlobalX >= FW || newGlobalY < 0 || newGlobalY >= FH) continue;
                    cellColorsIndexes[newGlobalY, newGlobalX] = (byte)curFigureColorIndex;
                }
            }
        }

        private bool CheckOverlap()
        {

            for (int dy = 0; dy < currentFigure.GetLength(0); dy++)
            {
                for (int dx = 0; dx < currentFigure.GetLength(1); dx++)
                {
                    if (currentFigure[dy, dx] == 0) continue;


                    int newGlobalX = curX + dx;
                    int newGlobalY = curY + dy;
                    if (newGlobalX < 0 || newGlobalX >= FW || newGlobalY < 0 || newGlobalY >= FH) continue;
                    if (cellColorsIndexes[newGlobalY, newGlobalX] != 0) return true;
                }
            }
            return false;
        }


        private bool GenerateNewFigure()
        {
            Random rnd = new Random();
            currentFigure = etalonFigures[rnd.Next(etalonFigures.Count)];
            curFigureColorIndex = 1 + rnd.Next(allColors.GetLength(0) - 1);
            curX = FW / 2 - 2;
            curY = FH - 2;
            if (CheckOverlap())
            {
                return false;
            }

            return true;
        }



        void BindShaders()
        {
            this.vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            this.vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);




            GL.BindVertexArray(0);

            string vertexShaderCode =
                @"
                #version 330 core
                layout (location=0) in vec3 aPosition;
                layout (location=1) in vec4 aColor;
                
                out vec4 vColor;
                void main()
                {
                    vColor = aColor;
                    gl_Position = vec4(aPosition, 1f);
                }
                ";
            string pixelShaderCode =
                @"
                #version 330 core
                in vec4 vColor;
                
                out vec4 pixelColor;                
                void main(){
                    pixelColor = vColor;
                }
                ";
            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);

            int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
            GL.CompileShader(pixelShaderHandle);

            this.programShaderHandle = GL.CreateProgram();
            GL.AttachShader(this.programShaderHandle, vertexShaderHandle);
            GL.AttachShader(this.programShaderHandle, pixelShaderHandle);
            GL.LinkProgram(this.programShaderHandle);

            GL.DetachShader(this.programShaderHandle, vertexShaderHandle);
            GL.DetachShader(this.programShaderHandle, pixelShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(pixelShaderHandle);
            GL.UseProgram(0);

            //GL.BindVertexArray(0);
            //GL.DeleteVertexArray(this.vertexArrayHandle);
        }
        protected override void OnLoad()
        {
            this.IsVisible = true;
            GL.ClearColor(Color4.Navy);

            GL.Enable(EnableCap.Blend);
            BindShaders();

            aTimer = new System.Timers.Timer(250);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Elapsed += TimerEvent;

            for(int k = 0; k < FH; ++k)
            {
                for(int j = 0; j < FW; ++j)
                {
                    ReColorCell(k, j, 0);
                }
            }
            NewScore();
            base.OnLoad();
        }


        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferHandle);
            GL.UseProgram(0);
            GL.DeleteProgram(programShaderHandle);
            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
            GL.Enable(EnableCap.Blend);


            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * vertices.Length, vertices);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(this.vertexArrayHandle);
            GL.UseProgram(programShaderHandle);

            GL.DrawArrays(PrimitiveType.Triangles, 0, FW * FH * 6 + LCD_TOTAL*LCD_H * LCD_W * SECTORS_PER_CIRCLE * 3);
            Context.SwapBuffers();
            GL.BindVertexArray(0);
            GL.Disable(EnableCap.Blend);
            
        }
    }
}
