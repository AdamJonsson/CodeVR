import Task from "../Types/Task";
import config from '../config.json';

export default async function getCurrentTaskStatus() {
    var response = await fetch(`http://${config.serverIP}:8999/api/current-task-status`);
    var taskStatus = await response.json();
    return taskStatus as TaskStatus;
}

export async function markCurrentTaskAsComplete()
{
    await fetch(`http://${config.serverIP}:8999/api/mark-current-task-completed`, { method: "POST"} );
}

export async function moveToNextTask()
{
    await fetch(`http://${config.serverIP}:8999/api/move-to-next-task`, { method: "POST"} );
}

export async function resetTaskStatusInBackend()
{
    await fetch(`http://${config.serverIP}:8999/api/reset`, { method: "POST"} );
}

export interface TaskStatus {
    task: Task,
    isCompleted: boolean,
    isLastTask: boolean
}

