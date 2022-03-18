interface Task {
    id: string,
    title: string,
    description: string,
    functionName: string,
    variables: string[],
    testCases: TestCases[]
}

interface TestCases
{
    inputs: string[],
    output: string
}

export const allTasks: Task[] = [
    {
        id: "test_task",
        title: "Task 0: Debug task",
        description: "The function 'test' should return true",
        functionName: "test",
        variables: [],
        testCases: [
            {
                inputs: [],
                output: "true"
            }
        ]
    },
    {
        id: "drive_task",
        title: "Task 1: Allowed to drive checker",
        description: "Given the age and the drunk state of a person, the function 'canDrive' should return true if the person can drive and false if not.",
        functionName: "canDrive",
        variables: ["age", "isDrunk"],
        testCases: [
            {
                inputs: ["10", "false"],
                output: "false"
            },
            {
                inputs: ["19", "true"],
                output: "false"
            },
            {
                inputs: ["20", "false"],
                output: "true"
            }
        ]
    },
    {
        id: "sum_task",
        title: "Task 2: Add numbers",
        description: "Given two numbers (x, y) the sum should be returned in the function 'addTwoNumbers'",
        functionName: "addTwoNumbers",
        variables: ["x", "y"],
        testCases: [
            {
                inputs: ["1", "1"],
                output: "2"
            },
            {
                inputs: ["-5", "5"],
                output: "0"
            },
            {
                inputs: ["0", "0"],
                output: "0"
            }
        ]
    }
]

export default Task;