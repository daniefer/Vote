using System;
using System.Threading.Tasks;
using System.Transactions;

namespace VoteApi.Data
{
	public class TransactionEnlistmentNotification : IEnlistmentNotification
	{
		public TransactionEnlistmentNotification(Func<Task> onCommit)
		{
			this.OnCommit = onCommit;
		}

		public Func<Task> OnCommit { get; }

		public void Commit(Enlistment enlistment)
		{
			OnCommit?.Invoke().ConfigureAwait(false).GetAwaiter().GetResult();
			enlistment.Done();
		}

		public void InDoubt(Enlistment enlistment)
		{
			enlistment.Done();
		}

		public void Prepare(PreparingEnlistment preparingEnlistment)
		{
			preparingEnlistment.Prepared();
		}

		public void Rollback(Enlistment enlistment)
		{
			enlistment.Done();
		}
	}
}
