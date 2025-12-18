// app/auth/login/hooks/useValidation.ts
import { useState, useCallback } from 'react';

export function useValidation(validator: (value: string) => boolean) {
  const [value, setValue] = useState('');
  const [isValid, setIsValid] = useState<boolean | null>(null);

  const handleChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    setValue(newValue);
    
    if (newValue === '') {
      setIsValid(null);
    } else {
      setIsValid(validator(newValue));
    }
  }, [validator]);

  return {
    value,
    setValue,
    isValid,
    handleChange,
  };
}