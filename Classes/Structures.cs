using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMES_Ziincheol.Classes
{
    public class Structures
    {
        public struct ExcelRows
        {
            //28개 항목
            public string orderDate;    //수주일
            public string projectNo;    //PROJECT NO
            public string orderNo;      //발주번호
            public string projectName;  //공사명
            public string materialNo;   //자재번호
            public string drawingNo;    //도면번호
            public string shipNo;       //호선No
            public string prodNameSpec; //품명규격
            public string unit;         //단위
            public int qty;             //수주수량

            public string requiredDate; //소요일
            public string prodDate;     //생산예정    
            public string class1;       //Class-1 
            public string class2;       //Class-2
            public string cls1InspDate; //Class-1 검사예정일
            public string cls2InspDate; //Class-2 검사예정일
            public string qmInspDate;   //QM검사예정일
            public string releaseDate;  //출고예정일
            public string remark;       //Remark1 
            public string engineType;   //엔진Type

            public string paint;        //페인트
            public string crvType;      //CRV Type
            public int crvUnitPrice;    //CRV 단가
            public int crvPrice;        //CRV 금액
            public string deliveryStore;//배송지
            public string takeoverDept; //인수부서명
            public string type;         //Type
            public string oringType;    //o-ring 구분
        }
        public struct MatRows
        {
            public string name;
            public string spec;
            public string unit;
        }

        public struct DictArgument
        {
            public int ProductRecipeID;
            public int WorkOrderID;
            public int ProductID;
            public string OrderComment;
            public string PackingPaper;
        }

        public struct CustomerArgument
        {
            public int nCustomerCode;
            public string sCustomerNumber;
            public string sCustomerName;
        }
        public struct OrderArgument
        {
            public int nOrderCode;
            public int nCustomerCode;
            public string sOrderNo;
            public string sCustomer;
        }

        public struct OrderItemArgument
        {
            public string sROrderItemNumber;
            public int nOrderItemCode;
            public int nProductItemCode;
            public int nCustomerCode;
        }
        public struct WorkStandardNoArgument
        {
            public string sFillerMetal;
            public string sFillerSize;
            public int nZone0Temp;
            public int nZone1Temp;
            public int nZone2Temp;
            public int nZone3Temp;
            public int nSpeed;
        }
        public struct SupplierArgument
        {
            public int nSupplierCode;
            public string sSupplierNumber;
            public string sSupplierName;
        }

        public struct ProcessArgument
        {
            public string sQuantity;
            public int nItemCode;
            public int nMp;
        }
        public struct mfgProcessArgument
        {
            public int nProcessCode;
            //public int nParentItem;
            public string sProcessNumber;
            public string sProcessName;
        }
    }
}
