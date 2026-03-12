using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace NewShop.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Oderdate { get; set; }
        public string No { get; set; }
        public string PhoneStatus { get; set; }
        public string SaleCustomer { get; set; }
        public string Step1 { get; set; }
        public string Step2 { get; set; }
        public string Step3 { get; set; }
        public string Step4 { get; set; }
        public string Step5 { get; set; }
        public string Status { get; set; }
        public string UserCreate { get; set; }
        public string SelltoCustomer { get; set; }
        public string SaleMan { get; set; }
        public string InsertedDate { get; set; }
        public string InsertedBy { get; set; }
        public string Document_Order { get; set; }
        public List<Product> Product_Grid { get; set; }
    }
    public class LoginUserViewModel
    {
        [Required]
        //[EmailAddress]
        [StringLength(150, MinimumLength = 10)]
        [Display(Name = "User: ")]
        public string Usre { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 2)]
        [Display(Name = "Password: ")]
        public string Password { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
    public class LoginuserInfo
    {
        public string OS { get; set; }
        public string Browser { get; set; }
        public string Ip_Addresss { get; set; }

    }
    public class CatProductGroup
    {
        public string Company { get; set; }
        public string ProductGroup { get; set; }
        public string ProductLine { get; set; }

    }
    public class LookupVehicle
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string SearchDescription { get; set; }
        public string CodeRelation { get; set; }
        public string YrStart { get; set; }
        public string YrEnd { get; set; }
        public string EngineType { get; set; }
        public string CC { get; set; }
        public string Picture { get; set; }
        public string sort { get; set; }
    }
    public class Stkgrop
    {
        public string STKGRP { get; set; }
        public string GRPNAM { get; set; }
        public string SEC { get; set; }
        public string PROD { get; set; }
        public string DEP { get; set; }
        public string COMPANY { get; set; }
    }


    public class logincutomer
    {
        public string EmpID { get; set; }
        public string company { get; set; }
        public string UsrID { get; set; }
        public string initials { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string EMail { get; set; }
        public string CUSCOD { get; set; }
        public string SUP { get; set; }
        public string UsrTyp { get; set; }
        public string SLMCOD { get; set; }
        public string SLMNAM { get; set; }
        public string PasswordExpiredDate { get; set; }
        public string DatetoExpire { get; set; }
        public string SLMPhone { get; set; }
        public string SalesCo { get; set; }
        public string SalesCoPhone { get; set; }
    }
    public class Brabdgrop
    {
        public string CODE { get; set; }
        public string Description { get; set; }

    }
    public class Segmentgrop
    {
        public string CODE { get; set; }
        public string segment { get; set; }
        public string SearchDes { get; set; }
        public string sort { get; set; }
    }
    public class Prod
    {
        public string CODE { get; set; }
        public string NAME { get; set; }


    }
    public class Searchitem
    {
        public string ItemNo { get; set; }
        public string Description { get; set; }

    }
    public class SearchitemDetailGetdata
    {
        public Searchitem val { get; set; }

    }
    public class PricelistpageingSearch
    {
        // [Display(Name = "Product")]
        //public int? Page { get; set; }
        public string CCheck_date { get; set; }
        public string PRCLST_NO { get; set; }
        public string PEOPLE { get; set; }
        public string CUSNAM { get; set; }
        public string SLMCOD { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string Brand { get; set; }
        public string STKGRP_PRC { get; set; }
        public string minord { get; set; }
        public string Promotion { get; set; }
        public string LastInvUnitPric { get; set; }
        public string LastInvDisc { get; set; }
        public string LastInvPrice { get; set; }
        public string LastInvdate { get; set; }
        public string TOTBAL { get; set; }
        public string BackOrder { get; set; }
        public string UOM { get; set; }
        public string PCDES { get; set; }
        public string Price0 { get; set; }
        public string Price { get; set; }
        public string SalePrice { get; set; }
        public string Special_Price { get; set; }
        public string spc_moq { get; set; }
        public string spc_start_date { get; set; }
        public string spc_end_date { get; set; }
        public string spc_remark { get; set; }
        public string spc_PRODAPP { get; set; }
        public string PRODNAM { get; set; }
        public string ORD_ID { get; set; }
        public string ORD_AMT { get; set; }
        public string ORD_DISCOUNT { get; set; }
        public string ORD_ExpectPrice { get; set; }
        public string ORD_Price { get; set; }
        public string ORD_QTY { get; set; }
        public string ORD_SalePrice { get; set; }
        public string ORDDAT { get; set; }
        public string ORD_LineNote { get; set; }
        public string company { get; set; }
        public string PromotionCode { get; set; }
        public string PromoDesc { get; set; }
        public string PromoPrice { get; set; }
        public string PromoMOQ { get; set; }
        public string PF { get; set; }
        public string Rack { get; set; }
        public string Rcw { get; set; }
        public string Totbck { get; set; }
        public string SPackUOM { get; set; }
        public string expired { get; set; }
        public string itemblock { get; set; }
        public string PATH { get; set; }
        public string Expected_Receipt_Date { get; set; }
        public string maxord { get; set; }
        public string Clearance { get; set; }
        public string ItemClass { get; set; }
        public string Intransit { get; set; }
        public string ReworkCanSales { get; set; }
        public string ReworkClearance { get; set; }
        public List<PricelistpageingSearch> PricelistpageingSearch_Grid { get; set; }
    }
    public class vehicle_PlusItem
    {
        public string Company { get; set; }
        public string STKCOD { get; set; }
        public string Description { get; set; }
        public string Stock { get; set; }
        public string EndPrice { get; set; }
        public string IMAGE_NAME { get; set; }

    }
    public class Listvehicle_PlusItem
    {
        public vehicle_PlusItem val { get; set; }
    }
    public class ListPagedList
    {
        public PricelistpageingSearch val { get; set; }
    }
    public class ItemListGetdata
    {
        public ItemOrdering val { get; set; }

    }
    public class ItemOrdering
    {
        public string CartID { get; set; }
        public string PRCLST_NO { get; set; }
        public string CUSCOD { get; set; }
        public string STKCOD { get; set; }
        public string Company { get; set; }
        public string STKDES { get; set; }
        public string STKGRP { get; set; }
        public string STKGRPNam { get; set; }
        public string MINORD { get; set; }
        public string Price { get; set; }
        public string SalePrice { get; set; }
        public string SpecialPrice { get; set; }
        public string ExpectPrice { get; set; }
        public string Qty { get; set; }
        public string User { get; set; }
        public string Amt { get; set; }
        public string ORDDAT { get; set; }
        public string LineNote { get; set; }
        public string Status { get; set; }
        public string Discount { get; set; }
        public int QtyAmt { get; set; }
        public string AmtQty { get; set; }
        public string AmtSalePrices { get; set; }
        public string TotalPrice { get; set; }
        public string TotalDiscount { get; set; }
        public string TotalAmt { get; set; }
        public string amtCredit { get; set; }
        public string AmtDiscount { get; set; }
        public string UOM { get; set; }
        public string SLMID { get; set; }
        public string PromotionDesc { get; set; }
        public string Promotion { get; set; }
        public string LastInvdate { get; set; }
        public string LastInvPrice { get; set; }
        public string InStock { get; set; }
        public string Item_Type { get; set; }
        public string PrcApproveBy { get; set; }
        public string Stock { get; set; }
        public string Backorder { get; set; }
        public string Type_Cal { get; set; }
        public string Special_Discount { get; set; }
        public string DiscountPercent { get; set; }
        public string ORDMOD_Type { get; set; }
        public string ORD_Type { get; set; }
        public string GenID { get; set; }
        public string Ready_Status { get; set; }
        public string maxord { get; set; }

        public string PrcRemark { get; set; }
        public string Promotion_Foc { get; set; }
        public string WH_Location { get; set; }
        public string KDC_QTY { get; set; }
        public string PDC_QTY { get; set; }
        public string AccessID { get; set; }
        public string Intransit { get; set; }
        public string InsertedBy { get; set; }

    }
    public class SLM
    {

        public string SLMCOD { get; set; }
        public string SLMNAM { get; set; }
    }
    public class CUS
    {
        public string CUSCOD { get; set; }
        public string CUSNAM { get; set; }
        public string PRO { get; set; }
        public string ADDR_01 { get; set; }
        public string ADDR_02 { get; set; }
        public string CUSTYP { get; set; }
        public string TAMCRLINE { get; set; }      // เปลี่ยนจาก AACCrlimit
        public string TAMBAL { get; set; }          // เปลี่ยนจาก AACBalance
        public string VELOXCRLINE { get; set; }    // เปลี่ยนจาก TACCrlimit
        public string VELOXBAL { get; set; }        // เปลี่ยนจาก TACBalance
        public string SLMCOD { get; set; }
        public string INACTIVE { get; set; }
        public string BLOCKED { get; set; }
        public string TAMPAYTRM { get; set; }      // เปลี่ยนจาก AACPAYTRM
        public string VELOXPAYTRM { get; set; }    // เปลี่ยนจาก TACPAYTRM
        public string TELNUM { get; set; }
        public string RATING { get; set; }
        public string Hierarchy1_Market_Segment { get; set; }
        public string Hierarchy2_Channel { get; set; }
        public string Hierarchy3_Bussiness_Type { get; set; }
    }
    public class shipto
    {
        public string customer { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string name2 { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
    }
    public class ItemListshipto
    {
        public shipto val { get; set; }

    }
    public class Transport_DenyK99
    {
        public string Code { get; set; }
        public string Name { get; set; }


    }
    public class ItemListTransport_DenyK99
    {
        public Transport_DenyK99 val { get; set; }

    }
    public class Itemordertype
    {
        public string ORD_Type { get; set; }
        public string ORD_TypeName { get; set; }
        public string ORD_Time { get; set; }
        public string DateCal { get; set; }

    }
    public class ItemConfirmPro
    {
        public string chk { get; set; }
        public string VQty { get; set; }
        public string VSTKCOD { get; set; }
        public string VCompany { get; set; }
        public string VSTKDES { get; set; }
        public string VSTKGRP { get; set; }
        public string VPrice { get; set; }
        public string VSalePrice { get; set; }
        public string VDiscount { get; set; }
        public string VAmt { get; set; }
        public string VORDDAT { get; set; }
        public string VLineNote { get; set; }
        public string VPromotion { get; set; }
        public string CUSCOD { get; set; }
        public string AmtQty { get; set; }
        public string AmtSalePrices { get; set; }
        public string TotalAmt { get; set; }
        public string Credit { get; set; }
        public string Ordertype { get; set; }
        public string AmtDiscount { get; set; }
        public string PRCLST_NO { get; set; }
        public string Uom { get; set; }
        public string SLMID { get; set; }
        public string PromotionDesc { get; set; }
        public string Type_Cal { get; set; }
        public string Special_Discount { get; set; }
        public string DiscountPercent { get; set; }
    }
    public class ItemConfirm
    {
        public string Vidorder { get; set; }
        public string VQty { get; set; }
        public string VSTKCOD { get; set; }
        public string Vstock { get; set; }
        public string Vtype { get; set; }
        public string VCompany { get; set; }
        public string VSTKDES { get; set; }
        public string VSTKGRP { get; set; }
        public string VPrice { get; set; }
        public string VSalePrice { get; set; }
        public string VDiscount { get; set; }
        public string VAmt { get; set; }
        public string VORDDAT { get; set; }
        public string VLineNote { get; set; }
        public string VPromotion { get; set; }
        public string CUSCOD { get; set; }
        public string AmtQty { get; set; }
        public string AmtSalePrices { get; set; }
        public string TotalAmt { get; set; }
        public string Credit { get; set; }
        public string Ordertype { get; set; }
        public string AmtDiscount { get; set; }
        public string PRCLST_NO { get; set; }
        public string Uom { get; set; }
        public string SLMID { get; set; }
        public string PromotionDesc { get; set; }
        public string Remark { get; set; }
        public string Vbackorder { get; set; }
        public string VGenID { get; set; }
        public string VWHLocation { get; set; }

    }

    public class SLMc
    {

        public string SumQty { get; set; }
        public string CountPN { get; set; }
        public string Countrow { get; set; }
        public string Status { get; set; }
    }
    public class Listslmcount
    {
        public SLMc val { get; set; }
    }
    public class DetailSLM
    {

        public string salmman { get; set; }
        public string salmmanname { get; set; }
        public string customer { get; set; }
        public string customername { get; set; }
        public string Countrow { get; set; }
        public string sumqty { get; set; }

    }
    public class ListsDetailSLM
    {
        public DetailSLM val { get; set; }
    }
    public class DetailApprvSLM
    {
        public string CUSCOD { get; set; }
        public string CUSNAM { get; set; }
        public string SLMCOD { get; set; }
        public string SLMNAM { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string qty { get; set; }
        public string Item_typ { get; set; }
    }
    public class SaleOrderList
    {

        public string RowNo { get; set; }
        public string ORD_ID { get; set; }
        public string ORD_DocNo { get; set; }
        public string ORD_Date { get; set; }
        public string ORD_TotalAmt { get; set; }
        public string QTY { get; set; }

        public string Picking { get; set; }
        public string Picking_Date { get; set; }
        public string Invoice { get; set; }
        public string Invoice_Date { get; set; }

        public string InsertDate { get; set; }
        public string DType { get; set; }
        public string Customer { get; set; }
        public string SlmCod { get; set; }
        public string Prn_ORD { get; set; }
        public string Order_Status { get; set; }
        public string PrintID { get; set; }
        public string StatusID { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string DeliveryDate { get; set; }
    }
    public class OrderListGetdata
    {
        public SaleOrderList val { get; set; }

    }
    public class OrderListDetailGetdata
    {
        public SaleOrderDetail val { get; set; }

    }
    public class SaleOrderDetail
    {
        public string RowNo { get; set; }
        public string ORD_DocNo { get; set; }
        public string chk { get; set; }
        public string VQty { get; set; }
        public string VSTKCOD { get; set; }
        public string VSTKDES { get; set; }
        public string VSTKGRP { get; set; }
        public string VPrice { get; set; }
        public string VSalePrice { get; set; }
        public string VDiscount { get; set; }
        public string VAmt { get; set; }
        public string VORDDAT { get; set; }
        public string VLineNote { get; set; }
        public string VPromotion { get; set; }
        public string CUSCOD { get; set; }
        public string AmtQty { get; set; }
        public string BckQty { get; set; }
        public string FlagBackOrder { get; set; }
        public string AmtSalePrices { get; set; }
        public string TotalAmt { get; set; }
        public string Credit { get; set; }
        public string Ordertype { get; set; }
        public string AmtDiscount { get; set; }
        public string PRCLST_NO { get; set; }
        public string Uom { get; set; }
        public string SLMID { get; set; }
        public string PromotionDesc { get; set; }
        public string Item_Type { get; set; }
        public string WH { get; set; }
        public string Round { get; set; }
        public string PIDate { get; set; }
        public string StartDelivery { get; set; }
        public string ExptoArrive { get; set; }
    }
    public class ItemListshop
    {
        public string Sop { get; set; }
        public string PRCLST_NO { get; set; }
        public string CUSCOD { get; set; }
        public string STKCOD { get; set; }
        public string PrHot { get; set; }
        public string Price0 { get; set; }
        public string Price { get; set; }
        public string SalePrice { get; set; }
        public string ExpectPrice { get; set; }
        public string Qty { get; set; }
        public string Moq { get; set; }
        public string User { get; set; }
        public string Amt { get; set; }
        public string LineNote { get; set; }
        public string ID { get; set; }
        public string UOM { get; set; }
        public string STKDES { get; set; }
        public string STKGRP { get; set; }
        public string company { get; set; }
        public string PromotionCode { get; set; }
        public string PromoDesc { get; set; }
        public string PromoPrice { get; set; }
        public string PromoMOQ { get; set; }
        public string Special_Price { get; set; }
        public string Spc_Remark { get; set; }
        public string Spc_moq { get; set; }
        public string LastInvPrice { get; set; }
        public string LastInvdate { get; set; }
        public string CSalePrice { get; set; }
        public string Qtybo { get; set; }
        public string PlcPrice { get; set; }
        public string SpcPrice { get; set; }

    }
    public class ItemFoc
    {
        public string chk { get; set; }
        public string VQty { get; set; }
        public string VSTKCOD { get; set; }
        public string VCompany { get; set; }
        public string VSTKDES { get; set; }
        public string VSTKGRP { get; set; }
        public string VPrice { get; set; }
        public string VSalePrice { get; set; }
        public string VDiscount { get; set; }
        public string VAmt { get; set; }
        public string VORDDAT { get; set; }
        public string VLineNote { get; set; }
        public string VPromotion { get; set; }
        public string CUSCOD { get; set; }
        public string AmtQty { get; set; }
        public string AmtSalePrices { get; set; }
        public string TotalAmt { get; set; }
        public string Credit { get; set; }
        public string Ordertype { get; set; }
        public string AmtDiscount { get; set; }
        public string PRCLST_NO { get; set; }
        public string Uom { get; set; }
        public string SLMID { get; set; }
        public string PromotionDesc { get; set; }
        public string VCUSCOD { get; set; }
        public string Backorderfoc { get; set; }
    }

    public class historyorder
    {
        public string Company { get; set; }
        public string CUSCOD { get; set; }
        public string CUSNAM { get; set; }
        public string CUSKEY { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string Qty { get; set; }
        public string Sp { get; set; }
        public string Amt { get; set; }
        public string Docdat { get; set; }
        public string NoofOrder { get; set; }
        public string TotalQty { get; set; }
        public string TotalAmt { get; set; }
        public string Qty01 { get; set; }
        public string Qty02 { get; set; }
        public string Qty03 { get; set; }
        public string Qty04 { get; set; }
        public string Qty05 { get; set; }
        public string Qty06 { get; set; }
        public string Qty07 { get; set; }
        public string Qty08 { get; set; }
        public string Qty09 { get; set; }
        public string Qty10 { get; set; }
        public string Qty11 { get; set; }
        public string Qty12 { get; set; }



    }

    public class HistoryorderList
    {
        public historyorder val { get; set; }

    }
    public class ItemListQno
    {
        public string QuatationNo { get; set; }
        public string Detail { get; set; }


    }
    public class ItemListdropList
    {
        public ItemListdrop val { get; set; }

    }
    public class ItemListdrop
    {
        public string No { get; set; }
        public string STKDES { get; set; }


    }
    public class Listloc
    {
        public string CUSCOD { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Remark { get; set; }
        public string CUSNAM { get; set; }
        public string ShipCode { get; set; }
        public string ShipStatus { get; set; }

    }
    public class CrmOrder
    {
        public string ScCrmNo { get; set; }
        public string ScCrm_DocNo { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string STKGRP { get; set; }
        public string Qty { get; set; }
        public string UnitPrice { get; set; }
        public string Amount { get; set; }
        public string StatusItem { get; set; }
        public string StatusItemName { get; set; }
        public string LineNote { get; set; }
        public string company { get; set; }
    }
    public class PricelistpageingSearchTemp
    {
        // [Display(Name = "Product")]
        public int? Page { get; set; }
        public string CCheck_date { get; set; }
        public string PRCLST_NO { get; set; }
        public string PEOPLE { get; set; }
        public string CUSNAM { get; set; }
        public string SLMCOD { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string STKGRP_PRC { get; set; }
        public string minord { get; set; }
        public string Promotion { get; set; }
        public string LastInvPrice { get; set; }
        public string LastInvdate { get; set; }
        public string TOTBAL { get; set; }
        public string BackOrder { get; set; }
        public string UOM { get; set; }
        public string PCDES { get; set; }
        public string Price0 { get; set; }
        public string Price { get; set; }
        public string SalePrice { get; set; }
        public string Special_Price { get; set; }
        public string spc_moq { get; set; }
        public string spc_start_date { get; set; }
        public string spc_end_date { get; set; }
        public string spc_remark { get; set; }
        public string spc_PRODAPP { get; set; }
        public string PRODNAM { get; set; }
        public string ORD_ID { get; set; }
        public string ORD_AMT { get; set; }
        public string ORD_DISCOUNT { get; set; }
        public string ORD_ExpectPrice { get; set; }
        public string ORD_Price { get; set; }
        public string ORD_QTY { get; set; }
        public string ORD_SalePrice { get; set; }
        public string ORDDAT { get; set; }
        public string ORD_LineNote { get; set; }
        public string company { get; set; }
        public string PromotionCode { get; set; }
        public string PromoDesc { get; set; }
        public string PromoPrice { get; set; }
        public string PromoMOQ { get; set; }
        public string Rcw { get; set; }
        public string Totbck { get; set; }
    }
    public class ListPagedListtemp
    {
        public PricelistpageingSearchTemp val { get; set; }
    }
    public class Crm
    {

        public string ScCrm_ID { get; set; }
        public string ScCrm_DocNo { get; set; }
        public string Document_Order { get; set; }
        public string ScCrm_Orderdate { get; set; }
        public string ScCrm_DocType { get; set; }
        public string ScCrm_RequeDelivery { get; set; }
        public string ScCrm_RequeDeliverytime { get; set; }
        public string ScCrm_ByCall { get; set; }
        public string ScCrm_PhoneStatus { get; set; }
        public string ScCrm_CUSCOD { get; set; }
        public string ScCrm_CUSNAM { get; set; }
        public string ScCrm_SLMCODE { get; set; }
        public string ScCrm_SLMNAM { get; set; }
        public string ScCrm_Status { get; set; }
        public string ScCrm_Step1 { get; set; }
        public string ScCrm_Step2 { get; set; }
        public string ScCrm_Step3 { get; set; }
        public string ScCrm_Step4 { get; set; }
        public string ScCrm_Step5 { get; set; }
        public string ScCrm_Linenote { get; set; }
        public string ScCrm_UserCreate { get; set; }
        public string ScCrm_Createdate { get; set; }
        public string ScCrm_Createdatetime { get; set; }
        public string ScCrm_UseClosed { get; set; }
        public string ScCrm_UseCloseddate { get; set; }
        public string ScCrm_UseCloseddatetime { get; set; }


    }
    public class CusDoc
    {
        public string Cus { get; set; }
        public string ScCrm_DocNo { get; set; }
        public string Document_Order { get; set; }
        public string Slm { get; set; }
        public string CusNam { get; set; }
        public string ORD_Date { get; set; }
        public string InsertBy { get; set; }
    }
    public class ListCusDoc
    {
        public CusDoc val { get; set; }
    }
    public class ItemListtemp
    {
        public string item_t { get; set; }
    }

    public class BackOrderbyItem
    {
        public string ckrow { get; set; }
        public string Qtybackrow { get; set; }
        public string Qtyclearrow { get; set; }
        public string Qtyleftoverkrow { get; set; }
        public string SaleOrder_No { get; set; }
        public string SaleOrder_Date { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string STKGRP { get; set; }
        public string Price { get; set; }
        public string SalePrice { get; set; }
        public string Discount { get; set; }
        public string Qty { get; set; }
        public string Amt { get; set; }
        public string LineNote { get; set; }
        public string CUSCOD { get; set; }
        public string CUSNAM { get; set; }
        public string SLMCOD { get; set; }
        public string SLMNAM { get; set; }
        public string TOTBAL { get; set; }
        public string PromotionCode { get; set; }
        public string ConfirmOrderQty { get; set; }
        public string SeqNum { get; set; }
        public string display { get; set; }
        public string ORDQTY { get; set; }
        public string TOTRES { get; set; }
        public string Allocated { get; set; }
        public string REVQTY { get; set; }
        public string InStock { get; set; }
        public string DLVDAT { get; set; }
        public string COM { get; set; }
        public string Promo { get; set; }
        public string Comment { get; set; }
        public string Totbck { get; set; }
        public string shipto { get; set; }
        public string CUSPO { get; set; }
        public string Servicepart { get; set; }
        public string OrdTyp { get; set; }
        public string inactive { get; set; }
        public string Blocked { get; set; }
        public string KDCQty { get; set; }
        public string PDCQty { get; set; }
        public string Rating { get; set; }
        public string PrdPstGrp { get; set; }

        public List<BackOrderbyItem> BackOrderbyItem_Grid { get; set; }
    }
    public class BackOrderbyDoc
    {
        public string ckrow { get; set; }
        public string Qtybackrow { get; set; }
        public string Qtyclearrow { get; set; }
        public string Qtyleftoverkrow { get; set; }
        public string SaleOrder_No { get; set; }
        public string SaleOrder_Date { get; set; }
        public string Qty { get; set; }
        public string Amt { get; set; }
        public string Inventory { get; set; }
        public string ConfirmOrderQty { get; set; }
        public string SeqNum { get; set; }
        public string Cuscod { get; set; }
        public string Slm { get; set; }
        public string CUSPO { get; set; }
        public List<BackOrderbyDoc> BackOrderbyDoc_Grid { get; set; }
    }
    public class MainPageModel
    {
        public BackOrderbyItem Model1 { get; set; }
        public BackOrderbyDoc Model2 { get; set; }
    }
    public class ItemBackOrder
    {
        public string DocNo { get; set; }
        public string SeqNo { get; set; }
        public string VQty { get; set; }
        public string VSTKCOD { get; set; }
        public string VSTKDES { get; set; }
        public string VSTKGRP { get; set; }
        public string VPrice { get; set; }
        public string VSalePrice { get; set; }
        public string VDiscount { get; set; }
        public string VAmt { get; set; }
        public string VORDDAT { get; set; }
        public string VPromotion { get; set; }
        public string CUSCOD { get; set; }
        public string User { get; set; }
        public string ValueQty { get; set; }
        public string Vlinenote { get; set; }
        public string promo { get; set; }

    }
    public partial class BackOrder
    {
        public string ORDBck_DocNo { get; set; }
        public string Company { get; set; }
        public string DocNo { get; set; }
        public string SEQNUM { get; set; }
        public System.DateTime DocDate { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string STKGRP { get; set; }
        public Nullable<int> ORDQTY { get; set; }
        public Nullable<int> REMQTY { get; set; }
        public Nullable<decimal> SELLPR { get; set; }
        public Nullable<int> ConfirmOrderQty { get; set; }
        public string CUSPO { get; set; }
        public string SLMCOD { get; set; }
        public string CUSCOD { get; set; }
        public Nullable<int> TOTBAL { get; set; }
        public Nullable<int> PrintID { get; set; }
        public Nullable<System.DateTime> InsertDate { get; set; }
        public string InsertBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public string linenote { get; set; }
        public Nullable<int> Prn_ORD { get; set; }
        public Nullable<System.DateTime> IF_Date { get; set; }
        public string IF_Status { get; set; }
        public string IF_Desc { get; set; }
        public string ORDMOD_Type { get; set; }
        public Nullable<System.DateTime> ORDMOD_Date { get; set; }
        public string ORDMOD_Time { get; set; }
        public string ORD_Type { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string Ship_Code { get; set; }
        public string Ship_Customer { get; set; }
        public string Transport_Code { get; set; }
        public string Customer_Po { get; set; }
        public string Remark { get; set; }
        public Nullable<int> TotalBack { get; set; }
        public Nullable<System.DateTime> ORDBck_Date { get; set; }
        public string PromotionCode { get; set; }
    }
    public class SaleOrder_History
    {
        public string CUSCOD { get; set; }
        public string CUSNAM { get; set; }
        public string STKCOD { get; set; }
        public string FullDES { get; set; }
        public string QTY { get; set; }
        public string SALPRICE { get; set; }
        public string AMT { get; set; }
        public string DISCOUNT { get; set; }
        public string ORDDAT { get; set; }
        public string SONUM { get; set; }
        public string PINUM { get; set; }
        public string INVNUM { get; set; }
        public string SLMCOD { get; set; }
        public List<SaleOrder_History> SaleOrder_History_Grid { get; set; }
    }
    public class Salesamt
    {
        public string Cuscod { get; set; }
        public string Company { get; set; }
        public string Amt { get; set; }
        public string YTD { get; set; }
        public string Jan { get; set; }
        public string Feb { get; set; }
        public string Mar { get; set; }
        public string Apr { get; set; }
        public string May { get; set; }
        public string Jun { get; set; }
        public string Jul { get; set; }
        public string Aug { get; set; }
        public string Sep { get; set; }
        public string Oct { get; set; }
        public string Nov { get; set; }
        public string Dec { get; set; }

    }
    public class Upload_History
    {
        public string Reference_No { get; set; }
        public string ID { get; set; }
        public string CUSCOD { get; set; }
        public string Company { get; set; }
        public string STKCOD { get; set; }
        public string Cus_STKCOD { get; set; }
        public string Qty { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }
        public string Status_Message { get; set; }
        public string Inserted_Date { get; set; }
        public string Inserted_By { get; set; }
        public string UOM { get; set; }
    }
    public class PhoneOtp
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
    public class SmsModels
    {
        public string Phone { get; set; }
        public string Text { get; set; }
        public string User { get; set; }
    }
    public class CustomerCredit
    {
        public string CUSCOD { get; set; }
        public string CUSNAME { get; set; }
        public string ADDR_01 { get; set; }
        public string ADDR_02 { get; set; }
        public string PRO { get; set; }
        public string CUSTYP { get; set; }
        public string LASIVC { get; set; }
        public string AACCRLINE { get; set; }
        public string AACBAL { get; set; }
        public string AACBALDue { get; set; }
        public string AACBilDue { get; set; }
        public string TACCRLINE { get; set; }
        public string TACBAL { get; set; }
        public string TACBALDue { get; set; }
        public string TACBilDue { get; set; }
        public string OMPCRLINE { get; set; }
        public string OMPBAL { get; set; }
        public string OMPBALDue { get; set; }
        public string OMPBilDue { get; set; }
        public string SLMCOD { get; set; }
        public string INACTIVE { get; set; }
        public string BLOCKED { get; set; }
        public string AACPAYTR { get; set; }
        public string TACPAYTR { get; set; }
        public string OMPPAYTR { get; set; }
        public string TELNUM { get; set; }
    }
    public class BackOrder_Notify
    {
        public string Company { get; set; }
        public string Document_No { get; set; }
        public string SONUM { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string Qty { get; set; }
        public string OrgQty { get; set; }
        public string SalePrice { get; set; }
        public string Amount { get; set; }
        public string SaleOrderDate { get; set; }
        public string DeliveryDate { get; set; }
        public string Note { get; set; }
        public string Brand { get; set; }
        public string ExpectedReceiptDate { get; set; } = string.Empty;
    }
    public class ListSendDelivery
    {
        public string Uid { get; set; }
        public string Docno { get; set; }
        public string Docdate { get; set; }
        public string Cusname { get; set; }
        public string Delivery { get; set; }
        public string DeliveryId { get; set; }
        public string UrlDocno { get; set; }
        public string UrlCuskey { get; set; }
        public string User { get; set; }
        public string Urldetail { get; set; }
    }
    public class ListSendDeliveryCount
    {
        public string sumOrderAll { get; set; }
        public string sumOrderCurrentDate { get; set; }
        public string sumOrderByMonth { get; set; }
        public string sumOrderStatus1 { get; set; }
        public string sumOrderStatus2 { get; set; }
        public string sumOrderStatus3 { get; set; }
        public string sumOrderStatus4 { get; set; }
        public string sumDateCurrent { get; set; }
    }
    public class ListOrderNotifySum
    {
        public string sumOrderCurrentDate { get; set; }
        public string sumOrderByMonth { get; set; }
        public string sumOrderStatus1 { get; set; }
        public string sumOrderStatus2 { get; set; }
        public string sumOrderStatus3 { get; set; }
        public string sumOrderStatus4 { get; set; }
    }
    public class DeliveryTrackNotify
    {
        public string ORD_DocNo { get; set; }
        public string CUSCOD { get; set; }
        public string NotifyID { get; set; }
        public string Notify { get; set; }
        public string StatusDate { get; set; }
        public string Order_Date { get; set; }
        public string ORD_TotalItem { get; set; }
        public string ORD_TotalQty { get; set; }
        public string ORD_TotalAmt { get; set; }
    }
    public class DeliveryTrackNotify_Detail
    {
        public string RowNo { get; set; }
        public string ORD_DocNo { get; set; }
        public string DType { get; set; }
        public string Item_Type { get; set; }
        public string ORD_STKCOD { get; set; }
        public string STKDES { get; set; }
        public string ORD_STKGRP { get; set; }
        public string ORD_Price { get; set; }
        public string ORD_SalePrice { get; set; }
        public string ORD_Date { get; set; }
        public string ORD_Discount { get; set; }
        public string ORD_Qty { get; set; }
        public string ORD_Amt { get; set; }
        public string BCK_Qty { get; set; }
        public string BackOrder { get; set; }
    }
    public class CusAmtMonth
    {
        public string Peiord { get; set; }
        public string CUSNAM { get; set; }
        public string Amount { get; set; }
    }
    public class Cusinv_Month
    {
        public string Company { get; set; }
        public string PSTDAT { get; set; }
        public string DUEDAT { get; set; }
        public string EXTDOC { get; set; }
        public string CUSNAM { get; set; }
        public string DOCNUM { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }

    }
    public class Cusinv_Item
    {
        public string Company { get; set; }
        public string PSTDAT { get; set; }
        public string DUEDAT { get; set; }
        public string EXTDOC { get; set; }
        public string CUSNAM { get; set; }
        public string DOCNUM { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string Qty { get; set; }
        public string Unit_Price { get; set; }
        public string Discount { get; set; }
        public string Amount { get; set; }
    }

    public class BillingDue
    {
        public string Company { get; set; }
        public string DocDat { get; set; }
        public string PstDat { get; set; }
        public string InvDue { get; set; }
        public string Invnum { get; set; }
        public string Amt { get; set; }
        public string BillNo { get; set; }
        public string BillDat { get; set; }
        public string BillDue { get; set; }
    }
    public class PromotionYearList
    {
        public string Year { get; set; }
        public string Reward_Amount { get; set; }
        public string Ticket { get; set; }
        public string Paid_Amount { get; set; }
        public string Waiting_Amount { get; set; }
        public string WHT { get; set; }
    }
    public class Promotion_CusList
    {
        public string Company { get; set; }
        public string Promotion_Code { get; set; }
        public string Promotion_Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Condition { get; set; }
        public string Invoice_Amount { get; set; }
        public string Invoice_Paid { get; set; }
        public string Remaining_Amount { get; set; }
        public string Reward { get; set; }
        public string Reward_Amt { get; set; }
        public string Received_By { get; set; }
        public string Received_Date { get; set; }
        public string WHT { get; set; }
        public string Status { get; set; }
    }
    public class Promotion_Detail
    {
        public string Promotion_code { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Company { get; set; }
        public string DOCNUM { get; set; }
        public string DOCDAT { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string PEOPLE { get; set; }
        public string QTY { get; set; }
        public string SP_LCY { get; set; }
        public string NET_LCY { get; set; }
        public string CMPCHK { get; set; }
        public string CMPLDAT { get; set; }
    }
    public class OrderHistoryByBrand
    {
        public string Company { get; set; }
        public string Stkcod { get; set; }
        public string Stkdes { get; set; }
        public string year_1_qty { get; set; }
        public string year_1_amt { get; set; }
        public string year_1_avg { get; set; }
        public string year_2_qty { get; set; }
        public string year_2_amt { get; set; }
        public string year_2_avg { get; set; }
        public string year_3_qty { get; set; }
        public string year_3_amt { get; set; }
        public string year_3_avg { get; set; }
        public string year_4_qty { get; set; }
        public string year_4_amt { get; set; }
        public string year_4_avg { get; set; }
        public string Brand { get; set; }
        public string Prclist { get; set; }
    }
    public class Notify_Detail
    {
        public string ORD_DocNo { get; set; }
        public string NotifyID { get; set; }
        public string Notify { get; set; }
        public string StatusDate { get; set; }
        public string PIDate { get; set; }
        public string ExpToArrive { get; set; }
        public string StartDelivery { get; set; }
        public string EndDelivery { get; set; }
        public string OrderDate { get; set; }
        public string ORD_TotalItem { get; set; }
        public string ORD_TotalQty { get; set; }
        public string ORD_TotalAmt { get; set; }
        public string Round { get; set; }
        public string WH { get; set; }
        public string ShipTo { get; set; }
        public string Item_Type { get; set; }
        public string ORD_STKCOD { get; set; }
        public string STKDES { get; set; }
        public string ORD_SalePrice { get; set; }
        public string ORD_Qty { get; set; }
        public string ORD_Amt { get; set; }
        public string BCK_Qty { get; set; }
        public string BackOrder { get; set; }
    }
    public class listPromotionCode
    {
        public string PromotionCode { get; set; }
        public string PromotionDes { get; set; }
    }
    public class listPromotionDetail
    {
        public string Promotion_Code { get; set; }
        public string Seq { get; set; }
        public string Type { get; set; }
        public string DesType { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
        public string Reward { get; set; }
        public string Reward_Percent { get; set; }
        public string Register_Expire_Date { get; set; }
        public string Count_reg { get; set; }
    }
    public class Warranty_Claim
    {
        public string REQ_NO { get; set; }
        public string CLM_NO_SUB { get; set; }
        public string REQ_DATE { get; set; }
        public string ReceiveDate { get; set; }
        public string CLM_COMPANY { get; set; }
        public string CUSCOD { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string Qty { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string Symptom { get; set; }
        public string Request { get; set; }
        public string DueDate { get; set; }
        public string Checking { get; set; }
        public string ApproveDate { get; set; }
        public string Status { get; set; }
        public string CS_No { get; set; }
        public string CS_Date { get; set; }

    }
    public class listCustomerRegister
    {
        public string Cuscode { get; set; }
        public string Cusname { get; set; }
        public string Slmcode { get; set; }
        public string FlagReg { get; set; }
        public string FlagApprove { get; set; }
        public int PackQty { get; set; }
        public int PackAmt { get; set; }
        public double SumPackAmt { get; set; }
        public string PackQtyPedingApprove { get; set; }
        public string Reason { get; set; }
    }
    public class listCustomerRegisterByCustomer
    {
        public string Cuscode { get; set; }
        public string Cusname { get; set; }
        public string Slmcode { get; set; }
        public string Promotion_Code { get; set; }
        public string Promotion_Sub { get; set; }
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int PackQty { get; set; }
        public int PackAmt { get; set; }
        public double SumPackAmt { get; set; }
        public string Promotion_Year { get; set; }
        public string FlagApprove { get; set; }
        public string PackQtyPedingApprove { get; set; }
        public string Reason { get; set; }
    }
    public class listCustomerRegisterSave
    {
        public string User { get; set; }
        public string SlmCode { get; set; }
        public string Cuscode { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionSeq { get; set; }
    }

    public class listCustomerApprove
    {
        public string Slmcode { get; set; }
        public string Cuscode { get; set; }
        public string Cusname { get; set; }
        public string Date_change { get; set; }
        public string Promotion_Code_Old { get; set; }
        public string Promotion_Code_old_Description { get; set; }
        public string Promotion_Sub_Old { get; set; }
        public string Description_Old { get; set; }
        public string Reward_Old { get; set; }
        public string PackQty_total_old { get; set; }
        public string SumPackQty_total_old { get; set; }
        public string Cost_Old { get; set; }
        public string PackQty_Old { get; set; }
        public string PackAmt_Old { get; set; }
        public string SumPackAmt_Old { get; set; }
        public string Promotion_Code_New { get; set; }
        public string Promotion_Sub_New { get; set; }
        public string Description_New { get; set; }
        public string Reward_New { get; set; }
        public string PackQty_total_new { get; set; }
        public string SumPackQty_total_new { get; set; }
        public string Cost_New { get; set; }
        public string PackQty_New { get; set; }
        public string PackAmt_New { get; set; }
        public string SumPackAmt_New { get; set; }
        public string Decrease_PackQty { get; set; }
        public string Decrease_SumPackAmt { get; set; }
        public string Reason { get; set; }
    }
    public class listSaveCustomerApprove
    {
        public string Cuscode { get; set; }
        public string Codeold { get; set; }
        public string Seqold { get; set; }
        public string Packqtyold { get; set; }
        public string Sumpackamtold { get; set; }
        public string Codenew { get; set; }
        public string Seqnew { get; set; }
        public string Packqtynew { get; set; }
        public string Sumpackamtnew { get; set; }
    }
    public class dashboardModel
    {
        public string Topic { get; set; }
        public string amt_qty1 { get; set; }
        public string amt_qty2 { get; set; }
        public string amt_qty3 { get; set; }
        public string amt_qty4 { get; set; }
        public string amt_qty5 { get; set; }
        public string amt_qty6 { get; set; }
        public string amt_qty7 { get; set; }
        public string amt_qty8 { get; set; }
        public string amt_qty9 { get; set; }
        public string amt_qty10 { get; set; }
        public string amt_qty11 { get; set; }
        public string amt_qty12 { get; set; }
        public string amt_qty13 { get; set; }
        public string amt_qty14 { get; set; }
        public string amt_qty15 { get; set; }
    }
    public class customerPortalUser
    {
        public string Cuscode { get; set; }
        public string Username { get; set; }
        public string CusName { get; set; }
    }

    public class StkCanSale
    {
        public string WH { get; set; }
        public string Company { get; set; }
        public string STKCOD { get; set; }
        public string STKDES { get; set; }
        public string STKGRP { get; set; }
        public string UOM { get; set; }
        public string ItemQty { get; set; }
        public string ReadyQty { get; set; }
        public string MO_Qty { get; set; }
        public string SO_Qty { get; set; }
        public string BuffQty { get; set; }
        public string RevQty { get; set; }
        public string InStock { get; set; }
        public string BckDue { get; set; }
    }
    public class ArticleDataProductBrand
    {
        public string MfrName { get; set; }
        public string ArticleNumber { get; set; }
        public string ArticleStatusDescription { get; set; }
        public List<GenericArticle> GenericArticles { get; set; }
        public List<OemNumber> OemNumbers { get; set; }
        public List<ArticleCriteria> ArticleCriteria { get; set; }
        public List<TradeNumberDetail> TradeNumberDetail { get; set; }
    }

    public class GenericArticle
    {
        public string genericArticleDescription { get; set; }
        public string assemblyGroupName { get; set; }
    }

    public class OemNumber
    {
        public string articleNumber { get; set; }
        public string mfrName { get; set; }
    }

    public class ArticleCriteria
    {
        public string criteriaDescription { get; set; }
        public string formattedValue { get; set; }
    }

    public class TradeNumberDetail
    {
        public string tradeNumber { get; set; }
    }
    public class listPromotionBeforeAfter
    {
        public string RowNumber { get; set; }
        public string Cuscode { get; set; }
        public string Cusname { get; set; }
        public string Slmcode { get; set; }
        public string PackQty { get; set; }
        public string PackAmt { get; set; }
        public string SumPackAmt { get; set; }
        public string InsertDate { get; set; }
        public string Reason { get; set; }
    }
    public class CustomerPromotionChangeViewModel
    {
        public listCustomerApprove CustomerData { get; set; }
        public List<listPromotionBeforeAfter> PromotionBeforeList { get; set; }
        public List<listPromotionBeforeAfter> PromotionAfterList { get; set; }
    }
    public class ListPMNotificate
    {
        public string Uid { get; set; }
        public string Topic { get; set; }
        public string Topiccod { get; set; }
        public string Stkcod { get; set; }
        public string Stkdes { get; set; }
        public string SPrice { get; set; }
        public string Qty { get; set; }
        public string Cuscod { get; set; }
        public string Cusname { get; set; }
        public string ApprvDate { get; set; }
        public string ApprvBy { get; set; }
        public string User { get; set; }
        public string Sta { get; set; }

    }

}