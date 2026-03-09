import {z} from 'zod';

const requiredString = (fieldName: string) => z.string({error: `${fieldName} is required`}).min(1, 
                                                                {error: `${fieldName} is required`})

export const activitySchema = z.object({

    title: requiredString('Title'),
    description: requiredString('Description'),
    category: requiredString('Category'),
    date: z.coerce.date({error: 'Date is required'}),
    location: z.object({
        venue: requiredString('Venue'),
        city: z.string().optional(),
        latitude: z.coerce.number(),
        longitude: z.coerce.number()
    })
})

/*
1. z.input<typeof schema>
This represents the type of the data before it is processed by Zod. It defines what the "raw" data should look like when you pass it into schema.parse().

Use Case: Use this for initial form values or data coming directly from an API.

Transformations: If you have a .transform(), the input type will show the original type.

2. z.infer<typeof schema> (Output)
This represents the type of the data after it has been successfully parsed and transformed. This is the "safe" version of the data that your application actually uses.

Use Case: Use this for function arguments, component props, and state variables after validation.

Transformations: If you transform a string to a Date, z.infer will correctly show the type as Date.
*/
 //export type ActivitySchema = z.input<typeof activitySchema>;
export type ActivitySchema = z.infer<typeof activitySchema>;