using Avro;
using Avro.Specific;

namespace MT.Contracts.Events.Order
{
	public partial class OrderCreated : ISpecificRecord
	{
		public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""OrderCreated"",""namespace"":""LetsGetChecked.Bus.Examples.Saga"",""fields"":[{""name"":""Id"",""doc"":""Event unique id"",""type"":{""type"":""string"",""logicalType"":""uuid""}},{""name"":""Source"",""doc"":""FQDN of the Aggregate producing this event"",""type"":""string""},{""name"":""SourceId"",""doc"":""Id of the Aggregate producing this event"",""type"":""string""},{""name"":""CreatedAt"",""doc"":""Event creation timestamp"",""type"":{""type"":""long"",""logicalType"":""timestamp-micros""}},{""name"":""Version"",""doc"":""Version of the aggregate that produced this event"",""type"":""long""},{""name"":""OrderId"",""doc"":""Id of the Order"",""type"":{""type"":""string"",""logicalType"":""uuid""}},{""name"":""ProductId"",""doc"":""Id of the Product"",""type"":{""type"":""string"",""logicalType"":""uuid""}},{""name"":""Quantity"",""doc"":""Quantity of the product being purchased"",""type"":""int""},{""name"":""PromotionId"",""doc"":""Id of the Promotion"",""type"":{""type"":""string"",""logicalType"":""uuid""}}]}");
		/// <summary>
		/// Event unique id
		/// </summary>
		private System.Guid _Id;
		/// <summary>
		/// FQDN of the Aggregate producing this event
		/// </summary>
		private string _Source;
		/// <summary>
		/// Id of the Aggregate producing this event
		/// </summary>1
		private string _SourceId;
		/// <summary>
		/// Event creation timestamp
		/// </summary>
		private System.DateTime _CreatedAt;
		/// <summary>
		/// Version of the aggregate that produced this event
		/// </summary>
		private long _Version;
		/// <summary>
		/// Id of the Order
		/// </summary>
		private System.Guid _OrderId;
		/// <summary>
		/// Id of the Product
		/// </summary>
		private System.Guid _ProductId;
		/// <summary>
		/// Quantity of the product being purchased
		/// </summary>
		private int _Quantity;
		/// <summary>
		/// Id of the Promotion
		/// </summary>
		private System.Guid _PromotionId;
		public virtual Schema Schema
		{
			get
			{
				return OrderCreated._SCHEMA;
			}
		}
		/// <summary>
		/// Event unique id
		/// </summary>
		public System.Guid Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				this._Id = value;
			}
		}
		/// <summary>
		/// FQDN of the Aggregate producing this event
		/// </summary>
		public string Source
		{
			get
			{
				return this._Source;
			}
			set
			{
				this._Source = value;
			}
		}
		/// <summary>
		/// Id of the Aggregate producing this event
		/// </summary>
		public string SourceId
		{
			get
			{
				return this._SourceId;
			}
			set
			{
				this._SourceId = value;
			}
		}
		/// <summary>
		/// Event creation timestamp
		/// </summary>
		public System.DateTime CreatedAt
		{
			get
			{
				return this._CreatedAt;
			}
			set
			{
				this._CreatedAt = value;
			}
		}
		/// <summary>
		/// Version of the aggregate that produced this event
		/// </summary>
		public long Version
		{
			get
			{
				return this._Version;
			}
			set
			{
				this._Version = value;
			}
		}
		/// <summary>
		/// Id of the Order
		/// </summary>
		public System.Guid OrderId
		{
			get
			{
				return this._OrderId;
			}
			set
			{
				this._OrderId = value;
			}
		}
		/// <summary>
		/// Id of the Product
		/// </summary>
		public System.Guid ProductId
		{
			get
			{
				return this._ProductId;
			}
			set
			{
				this._ProductId = value;
			}
		}
		/// <summary>
		/// Quantity of the product being purchased
		/// </summary>
		public int Quantity
		{
			get
			{
				return this._Quantity;
			}
			set
			{
				this._Quantity = value;
			}
		}
		/// <summary>
		/// Id of the Promotion
		/// </summary>
		public System.Guid PromotionId
		{
			get
			{
				return this._PromotionId;
			}
			set
			{
				this._PromotionId = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.Id;
			case 1: return this.Source;
			case 2: return this.SourceId;
			case 3: return this.CreatedAt;
			case 4: return this.Version;
			case 5: return this.OrderId;
			case 6: return this.ProductId;
			case 7: return this.Quantity;
			case 8: return this.PromotionId;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.Id = (System.Guid)fieldValue; break;
			case 1: this.Source = (System.String)fieldValue; break;
			case 2: this.SourceId = (System.String)fieldValue; break;
			case 3: this.CreatedAt = (System.DateTime)fieldValue; break;
			case 4: this.Version = (System.Int64)fieldValue; break;
			case 5: this.OrderId = (System.Guid)fieldValue; break;
			case 6: this.ProductId = (System.Guid)fieldValue; break;
			case 7: this.Quantity = (System.Int32)fieldValue; break;
			case 8: this.PromotionId = (System.Guid)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}
