export function ErrorState({ message }: { message: string }) {
  return <div className="error-card">{message}</div>;
}
