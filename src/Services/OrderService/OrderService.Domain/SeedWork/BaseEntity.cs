using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.SeedWork
{
    public abstract class BaseEntity
    {
        public virtual Guid Id { get; set; }

        public DateTime CreaeDate { get; set; }

        int? _requestHashCode;

        private List<INotification> _domainEvents;

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return Id == default;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is BaseEntity)) return false;

            if(ReferenceEquals(this, obj)) return true;

            if(GetType() != obj.GetType()) return false;

            BaseEntity item = (BaseEntity)obj;

            if (item.IsTransient() || IsTransient()) return false; else return item.Id == Id;

        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if(!_requestHashCode.HasValue)
                _requestHashCode = Id.GetHashCode() ^ 31; //XOR for Random distribution

                return _requestHashCode.Value;

            }
            else return base.GetHashCode();
        }

        public static bool operator ==(BaseEntity left,BaseEntity right)
        {
            if (Equals(left, null)) return Equals(right, null) ? true : false;
            else 
                return left.Equals(right);
        }

        public static bool operator !=(BaseEntity left,BaseEntity right)
        {
            return !(left == right);
        }
    }
}
