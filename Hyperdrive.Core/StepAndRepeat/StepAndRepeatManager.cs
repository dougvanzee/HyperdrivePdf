using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace Hyperdrive.Core.StepAndRepeat
{
    public static partial class StepAndRepeatManager
    {
        public static bool BusinessCard8Up(string SRC, string dest)
        {
            try
            {
                PdfDocument pdf = new PdfDocument(new PdfWriter(dest));

                PdfDocument sourcePdf = new PdfDocument(new PdfReader(SRC));

                PageSize nUpPageSize = PageSize.LETTER; // Letter Page Size

                for (int i = 1; i <= sourcePdf.GetNumberOfPages(); i++)
                {
                    //Original page

                    PdfPage origPage = sourcePdf.GetPage(i);

                    Rectangle pageSize = origPage.GetPageSize();
                    
                    float pageX = (pageSize.GetWidth() - pageSize.GetX()) / 72;
                    float pageY = (pageSize.GetHeight() - pageSize.GetY()) / 72;

                    float x = origPage.GetCropBox().GetX();
                    float y = origPage.GetCropBox().GetY();
                    float width = origPage.GetCropBox().GetWidth();
                    float height = origPage.GetCropBox().GetHeight();

                    float deltaX;
                    float deltaY;

                    if (pageX >= pageY)
                    {
                        deltaX = (288 - (width - x)) / 2;
                        deltaY = (180 - (height - y)) / 2;
                    }
                    else
                    {
                        deltaX = (180 - (width - x)) / 2;
                        deltaY = (288 - (height - y)) / 2;
                    }

                    origPage.SetCropBox(new Rectangle(x - deltaX, y - deltaY, width + (2 * deltaX), height + (2 * deltaY)));

                    // Setup Page

                    PdfFormXObject pageCopy = origPage.CopyAsFormXObject(pdf);

                    PdfPage page = pdf.AddNewPage(nUpPageSize);
                    PdfCanvas canvas = new PdfCanvas(page);

                    //N-up page

                    // Place horizontal BCs
                    if (pageX >= pageY)
                    {
                        // Horizontal BC with 0 rotation
                        if (origPage.GetRotation() == 0 || origPage.GetRotation() == 270)
                        {
                            for (int j = 1; j <= 8; j++)
                            {
                                int xx = j % 2 == 1 ? 18 : 306;
                                int yy = 180 * (int)(Math.Ceiling(Convert.ToDecimal(j) / 2)) - 144;

                                AffineTransform affineTransform = AffineTransform.GetTranslateInstance(deltaX + xx, deltaY + yy);
                                float[] matrix = new float[6];
                                affineTransform.GetMatrix(matrix);

                                canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                            }
                        }
                        // Horizontal BC with 180 rotation
                        else
                        {
                            for (int j = 1; j <= 8; j++)
                            {
                                int xx = j % 2 == 1 ? 18 : 306;
                                int yy = 180 * (int)(Math.Ceiling(Convert.ToDecimal(j) / 2)) - 144;

                                AffineTransform affineTransform = AffineTransform.GetRotateInstance(Math.PI);
                                affineTransform.Concatenate(AffineTransform.GetTranslateInstance(-612 + deltaX + xx, -792 + deltaY + yy));
                                float[] matrix = new float[6];
                                affineTransform.GetMatrix(matrix);

                                canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                            }
                        }
                    }
                    // Place vertical BCs
                    else
                    {
                        // Vertical BC with 90 rotation
                        if (origPage.GetRotation() == 0 || origPage.GetRotation() == 90)
                        {
                            for (int j = 1; j <= 8; j++)
                            {
                                int xx = j % 2 == 1 ? 18 : 306;
                                int yy = 180 * (int)(Math.Ceiling(Convert.ToDecimal(j) / 2)) - 144;

                                AffineTransform affineTransform = AffineTransform.GetRotateInstance(3 * Math.PI / 2);
                                affineTransform.Concatenate(AffineTransform.GetTranslateInstance(deltaY - yy - 180, xx + deltaX));
                                float[] matrix = new float[6];
                                affineTransform.GetMatrix(matrix);

                                canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                            }
                        }
                        // Vertical BC with 270 rotation
                        else
                        {
                            for (int j = 1; j <= 8; j++)
                            {
                                int xx = j % 2 == 1 ? 18 : 306;
                                int yy = 180 * (int)(Math.Ceiling(Convert.ToDecimal(j) / 2)) - 144;

                                AffineTransform affineTransform = AffineTransform.GetRotateInstance(Math.PI / 2);
                                affineTransform.Concatenate(AffineTransform.GetTranslateInstance(deltaY - yy - 180 + 792, xx + deltaX - 612));
                                float[] matrix = new float[6];
                                affineTransform.GetMatrix(matrix);

                                canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                            }
                        }
                    }
                }
                
                // close the documents

                pdf.Close();

                sourcePdf.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool BusinessCard9Up(string SRC, string dest)
        {
            try
            {
                PdfDocument pdf = new PdfDocument(new PdfWriter(dest));

                PdfDocument sourcePdf = new PdfDocument(new PdfReader(SRC));

                PageSize nUpPageSize = new PageSize(new Rectangle(0, 0, 864, 648)); // Letter Page Size

                for (int i = 1; i <= sourcePdf.GetNumberOfPages(); i++)
                {
                    //Original page

                    PdfPage origPage = sourcePdf.GetPage(i);

                    Rectangle pageSize = origPage.GetPageSize();

                    float pageX = (pageSize.GetWidth() - pageSize.GetX()) / 72;
                    float pageY = (pageSize.GetHeight() - pageSize.GetY()) / 72;

                    float x = origPage.GetCropBox().GetX();
                    float y = origPage.GetCropBox().GetY();
                    float width = origPage.GetCropBox().GetWidth();
                    float height = origPage.GetCropBox().GetHeight();

                    float deltaX;
                    float deltaY;

                    if (pageX >= pageY)
                    {
                        deltaX = (279 - (width - x)) / 2;
                        deltaY = (171 - (height - y)) / 2;
                    }
                    else
                    {
                        deltaX = (171 - (width - x)) / 2;
                        deltaY = (279 - (height - y)) / 2;
                    }

                    origPage.SetCropBox(new Rectangle(x - deltaX, y - deltaY, width + (2 * deltaX), height + (2 * deltaY)));

                    // Setup Page

                    PdfFormXObject pageCopy = origPage.CopyAsFormXObject(pdf);

                    PdfPage page = pdf.AddNewPage(nUpPageSize);
                    PdfCanvas canvas = new PdfCanvas(page);

                    //N-up page

                    // Place horizontal BCs
                    if (pageX >= pageY)
                    {
                        // Horizontal BC with 0 rotation
                        if (origPage.GetRotation() == 0 || origPage.GetRotation() == 270)
                        {
                            for (int j = 1; j <= 3; j++)
                            {
                                for (int k = 1; k <= 3; k++)
                                {
                                    float xx = 0;
                                    if (j == 1) xx = 13.6912f;
                                    if (j == 2) xx = 292.6912f;
                                    if (j == 3) xx = 571.6912f;

                                    float yy = 0;
                                    if (k == 1) yy = 42.5f;
                                    if (k == 2) yy = 238.5f;
                                    if (k == 3) yy = 434.5f;

                                    AffineTransform affineTransform = AffineTransform.GetTranslateInstance(deltaX + xx, deltaY + yy);
                                    float[] matrix = new float[6];
                                    affineTransform.GetMatrix(matrix);

                                    canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                                }
                            }
                        }
                        // Horizontal BC with 180 rotation
                        else
                        {
                            for (int j = 1; j <= 8; j++)
                            {
                                int xx = j % 2 == 1 ? 18 : 306;
                                int yy = 180 * (int)(Math.Ceiling(Convert.ToDecimal(j) / 2)) - 144;

                                AffineTransform affineTransform = AffineTransform.GetRotateInstance(Math.PI);
                                affineTransform.Concatenate(AffineTransform.GetTranslateInstance(-612 + deltaX + xx, -792 + deltaY + yy));
                                float[] matrix = new float[6];
                                affineTransform.GetMatrix(matrix);

                                canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                            }
                        }
                    }
                    // Place vertical BCs
                    else
                    {
                        // Vertical BC with 90 rotation
                        if (origPage.GetRotation() == 0 || origPage.GetRotation() == 90)
                        {
                            for (int j = 1; j <= 8; j++)
                            {
                                int xx = j % 2 == 1 ? 18 : 306;
                                int yy = 180 * (int)(Math.Ceiling(Convert.ToDecimal(j) / 2)) - 144;

                                AffineTransform affineTransform = AffineTransform.GetRotateInstance(3 * Math.PI / 2);
                                affineTransform.Concatenate(AffineTransform.GetTranslateInstance(deltaY - yy - 180, xx + deltaX));
                                float[] matrix = new float[6];
                                affineTransform.GetMatrix(matrix);

                                canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                            }
                        }
                        // Vertical BC with 270 rotation
                        else
                        {
                            for (int j = 1; j <= 8; j++)
                            {
                                int xx = j % 2 == 1 ? 18 : 306;
                                int yy = 180 * (int)(Math.Ceiling(Convert.ToDecimal(j) / 2)) - 144;

                                AffineTransform affineTransform = AffineTransform.GetRotateInstance(Math.PI / 2);
                                affineTransform.Concatenate(AffineTransform.GetTranslateInstance(deltaY - yy - 180 + 792, xx + deltaX - 612));
                                float[] matrix = new float[6];
                                affineTransform.GetMatrix(matrix);

                                canvas.AddXObjectWithTransformationMatrix(pageCopy, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                            }
                        }
                    }
                }

                // close the documents

                pdf.Close();

                sourcePdf.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        


    }
}
