import type { OrderStatusEvent } from '../types/orderflow';

export function StatusTimeline({ events }: { events: OrderStatusEvent[] }) {
  if (events.length === 0) {
    return <p className="muted">No status events yet.</p>;
  }

  return (
    <div className="timeline">
      {events.map((event) => (
        <div className="timeline-item" key={event.id}>
          <div className="timeline-dot" />
          <div>
            <div className="timeline-title">
              {event.oldStatus ? `${event.oldStatus} → ${event.newStatus}` : event.newStatus}
            </div>
            <div className="timeline-note">{event.note}</div>
            <div className="timeline-time">{new Date(event.createdAt).toLocaleString()}</div>
          </div>
        </div>
      ))}
    </div>
  );
}
