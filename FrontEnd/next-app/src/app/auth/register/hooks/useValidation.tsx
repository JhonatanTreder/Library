import { useState } from "react";

export function useValidation(validator: (value: string) => boolean){
    const [value, setValue] = useState("")
    const [isValid, setValid] = useState<boolean | null>(null)

    function handleChange(event: React.ChangeEvent<HTMLInputElement>){
        const newValue = event.target.value
        
        setValue(newValue)
        setValid(validator(newValue))
    }

    return { value, isValid, handleChange}
}