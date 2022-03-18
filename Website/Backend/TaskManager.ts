import Task, { allTasks } from "./tasks";

export default class TaskManager 
{
    private currentTaskIndex: number = 0;
    private currentTaskCompleted: boolean = false;

    public get currentActiveTask() {
        return allTasks[this.currentTaskIndex];
    }

    public get currentTaskStatus(): TaskStatus {
        return {
            task: this.currentActiveTask,
            isCompleted: this.currentTaskCompleted,
            isLastTask: this.isLastTask,
        }
    }

    private get isLastTask() {
        return this.currentTaskIndex == allTasks.length - 1;
    }

    public moveToNextTask() {
        if (this.isLastTask) return;
        this.currentTaskIndex++;
        this.currentTaskCompleted = false;
    }
    
    public markCurrentLevelComplete() {
        this.currentTaskCompleted = true;
    }
}

interface TaskStatus {
    task: Task,
    isCompleted: boolean,
    isLastTask: boolean,
}