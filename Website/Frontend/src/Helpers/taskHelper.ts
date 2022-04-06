import Task from "../Types/Task";
import config from '../config.json';

const baseAdress = `http://${config.serverIP}:8999`;

export default async function getCurrentTaskStatus() {
    var response = await fetch(`${baseAdress}/api/current-task-status`);
    var taskStatus = await response.json();
    return taskStatus as TaskStatus;
}

export async function updateCurrentTaskStatus(isCompleted: boolean, failedTest: FailedTest | null, currentOutput: string)
{
    const data = new URLSearchParams();
    data.append('data', JSON.stringify({
        isCompleted: isCompleted,
        failedTest: failedTest,
        currentOutput: currentOutput
    }));

    await fetch(`${baseAdress}/api/mark-current-task-completed`, {
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: data, 
        method: "POST"
    });
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

interface FailedTest {
    inputs: string, 
    output: string
}