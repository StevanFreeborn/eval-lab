import { inject, InjectionKey } from 'vue';

export function useService<T>(serviceKey: InjectionKey<T>): T {
  const service = inject(serviceKey);

  if (service === undefined) {
    throw new Error(`Service not found: ${serviceKey.toString()}`);
  }

  return service;
}
