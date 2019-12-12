using Shared_Utility;
using Shared_Utility.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;

namespace EngineViewer._3D.TCP
{
    public class JStruct: Shared_Utility.Types.JStruct
    {
        public JStruct FromServer(Net.ResponseBytes resp)
        {
            try
            {
                var js = this.JDeserializemyData(Encoding.ASCII.GetString(File.ReadAllBytes(resp.SavedDataPath)));
                return js;
            }
            catch (Exception ex)
            {
                ex.Log("Error Reading json from Server");
            }

            return null;
        }
    }
}
