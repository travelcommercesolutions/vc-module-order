using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrderModule.Data.Model
{
    public class CustomerOrderEntity : OperationEntity
    {
        [Required]
        [StringLength(64)]
        public string CustomerId { get; set; }
        [StringLength(255)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(64)]
        public string StoreId { get; set; }
        [StringLength(255)]
        public string StoreName { get; set; }

        [StringLength(64)]
        public string ChannelId { get; set; }
        [StringLength(64)]
        public string OrganizationId { get; set; }
        [StringLength(255)]
        public string OrganizationName { get; set; }

        [StringLength(64)]
        public string EmployeeId { get; set; }
        [StringLength(255)]
        public string EmployeeName { get; set; }

        [StringLength(64)]
        public string SubscriptionId { get; set; }
        [StringLength(64)]
        public string SubscriptionNumber { get; set; }

        public bool IsPrototype { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal TaxTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal Total { get; set; }
        [Column(TypeName = "Money")]
        public decimal SubTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal SubTotalWithTax { get; set; }
        [Column(TypeName = "Money")]
        public decimal ShippingTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal ShippingTotalWithTax { get; set; }
        [Column(TypeName = "Money")]
        public decimal PaymentTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal PaymentTotalWithTax { get; set; }
        [Column(TypeName = "Money")]
        public decimal HandlingTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal HandlingTotalWithTax { get; set; }
        [Column(TypeName = "Money")]
        public decimal DiscountTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal DiscountTotalWithTax { get; set; }
        [StringLength(16)]
        public string LanguageCode { get; set; }
        public decimal TaxPercentRate { get; set; }

        [StringLength(128)]
        public string ShoppingCartId { get; set; }

        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; } = new NullCollection<TaxDetailEntity>();
        public virtual ObservableCollection<AddressEntity> Addresses { get; set; } = new NullCollection<AddressEntity>();
        public virtual ObservableCollection<PaymentInEntity> InPayments { get; set; } = new NullCollection<PaymentInEntity>();

        public virtual ObservableCollection<LineItemEntity> Items { get; set; } = new NullCollection<LineItemEntity>();
        public virtual ObservableCollection<ShipmentEntity> Shipments { get; set; } = new NullCollection<ShipmentEntity>();

        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; } = new NullCollection<DiscountEntity>();


        public override OrderOperation ToModel(OrderOperation operation)
        {
            var order = operation as CustomerOrder;
            if (order == null)
                throw new ArgumentException(@"operation argument must be of type CustomerOrder", nameof(operation));

            order.Discounts = Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();
            order.Items = Items.Select(x => x.ToModel(AbstractTypeFactory<LineItem>.TryCreateInstance())).ToList();
            order.Addresses = Addresses.Select(x => x.ToModel(AbstractTypeFactory<Address>.TryCreateInstance())).ToList();
            order.Shipments = Shipments.Select(x => x.ToModel(AbstractTypeFactory<Shipment>.TryCreateInstance())).OfType<Shipment>().ToList();
            order.InPayments = InPayments.Select(x => x.ToModel(AbstractTypeFactory<PaymentIn>.TryCreateInstance())).OfType<PaymentIn>().ToList();
            order.TaxDetails = TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();

            base.ToModel(order);

            Sum = order.Total;

            return order;
        }

        public override OperationEntity FromModel(OrderOperation operation, PrimaryKeyResolvingMap pkMap)
        {
            var order = operation as CustomerOrder;
            if (order == null)
                throw new ArgumentException(@"operation argument must be of type CustomerOrder", nameof(operation));

            base.FromModel(order, pkMap);

            if (order.Addresses != null)
            {
                Addresses = new ObservableCollection<AddressEntity>(order.Addresses.Select(x => AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(x)));
            }

            if (order.Items != null)
            {
                Items = new ObservableCollection<LineItemEntity>(order.Items.Select(x => AbstractTypeFactory<LineItemEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            if (order.Shipments != null)
            {
                Shipments = new ObservableCollection<ShipmentEntity>(order.Shipments.Select(x => AbstractTypeFactory<ShipmentEntity>.TryCreateInstance().FromModel(x, pkMap)).OfType<ShipmentEntity>());
                //Link shipment item with order lineItem 
                foreach (var shipmentItemEntity in Shipments.SelectMany(x => x.Items))
                {
                    shipmentItemEntity.LineItem = Items.FirstOrDefault(x => x.ModelLineItem == shipmentItemEntity.ModelLineItem);
                }
            }

            if (order.InPayments != null)
            {
                InPayments = new ObservableCollection<PaymentInEntity>(order.InPayments.Select(x => AbstractTypeFactory<PaymentInEntity>.TryCreateInstance().FromModel(x, pkMap)).OfType<PaymentInEntity>());
            }

            if (order.Discounts != null)
            {
                Discounts = new ObservableCollection<DiscountEntity>(order.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }

            if (order.TaxDetails != null)
            {
                TaxDetails = new ObservableCollection<TaxDetailEntity>(order.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }

            Sum = order.Total;

            return this;
        }

        public override void Patch(OperationEntity operation)
        {
            var target = operation as CustomerOrderEntity;
            if (target == null)
                throw new ArgumentException(@"operation argument must be of type CustomerOrderEntity", nameof(operation));

            target.CustomerId = CustomerId;
            target.CustomerName = CustomerName;
            target.StoreId = StoreId;
            target.StoreName = StoreName;
            target.OrganizationId = OrganizationId;
            target.OrganizationName = OrganizationName;
            target.EmployeeId = EmployeeId;
            target.EmployeeName = EmployeeName;
            target.DiscountAmount = DiscountAmount;
            target.Total = Total;
            target.SubTotal = SubTotal;
            target.SubTotalWithTax = SubTotalWithTax;
            target.ShippingTotal = ShippingTotal;
            target.ShippingTotalWithTax = ShippingTotalWithTax;
            target.PaymentTotal = PaymentTotal;
            target.PaymentTotalWithTax = PaymentTotalWithTax;
            target.HandlingTotal = HandlingTotal;
            target.HandlingTotalWithTax = HandlingTotalWithTax;
            target.DiscountTotal = DiscountTotal;
            target.DiscountTotalWithTax = DiscountTotalWithTax;
            target.DiscountAmount = DiscountAmount;
            target.TaxTotal = TaxTotal;
            target.IsPrototype = IsPrototype;
            target.SubscriptionNumber = SubscriptionNumber;
            target.SubscriptionId = SubscriptionId;
            target.LanguageCode = LanguageCode;
            target.TaxPercentRate = TaxPercentRate;

            if (!Addresses.IsNullCollection())
            {
                Addresses.Patch(target.Addresses, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }

            if (!Shipments.IsNullCollection())
            {
                Shipments.Patch(target.Shipments, (sourceShipment, targetShipment) => sourceShipment.Patch(targetShipment));
            }

            if (!Items.IsNullCollection())
            {
                Items.Patch(target.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }

            if (!InPayments.IsNullCollection())
            {
                InPayments.Patch(target.InPayments, (sourcePayment, targetPayment) => sourcePayment.Patch(targetPayment));
            }

            if (!Discounts.IsNullCollection())
            {
                var discountComparer = AnonymousComparer.Create((DiscountEntity x) => x.PromotionId);
                Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }
            if (!TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AnonymousComparer.Create((TaxDetailEntity x) => x.Name);
                TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }

            base.Patch(operation);
        }
    }
}
