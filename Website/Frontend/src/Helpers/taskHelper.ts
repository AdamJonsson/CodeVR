import Task from "../Types/Task";
import config from '../config.json';

const baseAdress = `http://${config.serverIP}:8999`;

export default async function getCurrentTaskStatus() {
    var response = await fetch(`${baseAdress}/api/current-task-status`);
    var taskStatus = await response.json();
    return taskStatus as TaskStatus;
}

export async function markCurrentTaskAsComplete()
{
    await fetch(`${baseAdress}/api/mark-current-task-completed`, { method: "POST"} );
}

export async function moveToNextTask()
{
    await fetch(`${baseAdress}/api/move-to-next-task`, { method: "POST"} );
}

export async function resetTaskStatusInBackend()
{
    await fetch(`${baseAdress}/api/reset`, { method: "POST"} );
}

export interface TaskStatus {
    task: Task,
    isCompleted: boolean,
    isLastTask: boolean
}

