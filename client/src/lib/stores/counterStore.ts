import { makeAutoObservable } from "mobx";

export default class CounterStore {
    title = "Counter store";
    // Observable state
    // MobX watches these values. When they change,
    // React components using this store will update automatically.
    count = 73;
    events: string[] = [
        `Initial count is ${this.count}`
    ];

    constructor() {
        makeAutoObservable(this)
    }
    // Action
    // This function changes the observable state (count and events)
    increment = (amount = 1) => {
        this.count += amount;
        this.events.push(`Incremented by ${amount} - count is now ${this.count}`);
    }
    // Action
    // Another function that modifies the observable state
    decrement = (amount = 1) => {
        this.count -= amount;
        this.events.push(`Decremented by ${amount} - count is now ${this.count}`);
    }
    // Computed property
    // This value is calculated from other observable data (events)
    // MobX updates it automatically when events change
    get eventCount() {
        return this.events.length;
    }
}