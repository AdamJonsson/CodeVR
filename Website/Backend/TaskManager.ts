import Task, { allTasks, TestCases } from "./tasks";

export default class TaskManager 
{
    private currentTaskIndex: number = 1;
    private currentTaskCompleted: boolean = false;
    private currentOutput: string = "TEST";
    private failedTest: FailedTest | null = null;

    public get currentActiveTask() {
        return allTasks[this.currentTaskIndex];
    }

    public get currentTaskStatus(): TaskStatus {
        return {
            task: this.currentActiveTask,
            isCompleted: this.currentTaskCompleted,
            isLastTask: this.isLastTask,
            currentOutput: this.currentOutput,
            failedTest: this.failedTest,
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
    
    public updateTaskStatus(isCompleted: boolean, failedTest: FailedTest | null, currentOutput: string) {
        this.currentTaskCompleted = isCompleted;
        this.failedTest = failedTest;
        this.currentOutput = currentOutput;
    }
}

interface TaskStatus {
    task: Task,
    isCompleted: boolean,
    isLastTask: boolean,
    failedTest: FailedTest | null,
    currentOutput: string
}

interface FailedTest {
    inputs: string, 
    output: string
}