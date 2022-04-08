interface Task {
    id: string,
    title: string,
    description: string,
    functionName: string,
    variables: string[],
    testCases: TestCases[]
}

export interface TestCases
{
    inputs: string[],
    output: string
}

export const allTasks: Task[] = [
    {
        id: "welcome_task_1",
        title: "Task 0: Welcome",
        description: "The function 'SayHi' should return 'Hello World'. Do this by connecting the exiting blocks.",
        functionName: "SayHi",
        variables: [],
        testCases: [
            {
                inputs: [],
                output: "Hello World"
            }
        ]
    },
    {
        id: "welcome_task_2",
        title: "Task 1: Welcome Code",
        description: "Now, the function 'SayCode' should return 'Hello Code'. Do this by editing the text.",
        functionName: "SayCode",
        variables: [],
        testCases: [
            {
                inputs: [],
                output: "Hello Code"
            }
        ]
    },
    {
        id: "math_task_1",
        title: "Task 2: Math",
        description: "The function 'SolveMathProblem' should return the value of 4354 x 8734. Do this by using the arithmetic block",
        functionName: "SolveMathProblem",
        variables: [],
        testCases: [
            {
                inputs: [],
                output: "38027836"
            }
        ]
    },
    {
        id: "math_task_2",
        title: "Task 3: Math",
        description: "The function 'SolveMathProblem' should return the value the sum of variables 'X' and 'Y'. Do this by using the arithmetic block",
        functionName: "SolveMathProblem",
        variables: ["x", "y"],
        testCases: [
            {
                inputs: ["-1", "1"],
                output: "0"
            },
            {
                inputs: ["0", "0"],
                output: "0"
            },
            {
                inputs: ["4", "6"],
                output: "10"
            }
        ]
    },
    {
        id: "math_task_3",
        title: "Task 4: Math",
        description: "The function 'SolveMathProblem' should return the value the multiplication of variables 'X' and 'Y'. You need to add the new blocks from the menu to the left.",
        functionName: "SolveMathProblem",
        variables: ["x", "y"],
        testCases: [
            {
                inputs: ["-1", "1"],
                output: "-1"
            },
            {
                inputs: ["0", "0"],
                output: "0"
            },
            {
                inputs: ["4", "6"],
                output: "24"
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