using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSGeo.GDAL;
using System.Drawing;
using System.Drawing.Imaging;

namespace HMSGDAL
{
    class HMSRaster
    {
        
        public Bitmap ReadRaster(out string errorMsg, string gdalName)
        {
            errorMsg = "";
            try
            {
                Dataset ds = Gdal.Open(gdalName, Access.GA_ReadOnly);
                if (ds == null) { errorMsg = "ERROR: Unable to open raster dataset."; return null; }
                Driver drv = ds.GetDriver();
                if (drv == null) { errorMsg = "ERROR: Unable to open raster driver."; return null; }

                Band band = ds.GetRasterBand(1);
                int width = band.XSize;
                int height = band.YSize;

                Bitmap bMap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                BitmapData bMapData = bMap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
                try
                {
                    int stride = bMapData.Stride;
                    IntPtr buf = bMapData.Scan0;
                    band.ReadRaster(0, 0, width, height, buf, width, height, DataType.GDT_Byte, 1, stride);
                }
                finally
                {
                    bMap.UnlockBits(bMapData);
                    
                }
                return bMap;
            }
            catch
            { }
            return null;
        }



    }
}
