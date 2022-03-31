import { TaskStatus } from '../Helpers/taskHelper';
import useWebsocketConnection from './useWebsocketConnection';

function useServerTaskStatus(): [TaskStatus | null] {
  const [taskStatusFromServer, connectionStatus] = useWebsocketConnection<TaskStatus>("taskStatus");
  return [taskStatusFromServer];
}

export default useServerTaskStatus;