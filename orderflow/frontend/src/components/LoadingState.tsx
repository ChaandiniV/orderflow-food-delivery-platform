export function LoadingState({ message = 'Loading...' }: { message?: string }) {
  return (
    <div className="loading-card">
      <div className="spinner" />
      <p>{message}</p>
    </div>
  );
}
