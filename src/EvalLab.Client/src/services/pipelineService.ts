import { InjectionKey } from 'vue';
import {
  createGenericService,
  Entity,
  GenericService,
  makeRequest,
  result,
  Result,
} from './shared';

type NewPipeline = {
  name: string;
  description: string;
  endpoint: string;
};

export type Pipeline = NewPipeline & Entity;

export type Run = {
  id: string;
  input: string;
  output: string;
  createdDate: Date;
};

type PipelinesService = GenericService<NewPipeline, Pipeline> & {
  run(id: string, input: string): Promise<Result<Run>>;
};

type PipelinesServiceKeyType = InjectionKey<PipelinesService>;

export const PipelinesServiceKey: PipelinesServiceKeyType = Symbol('PipelinesService');

const BASE_URL = '/api/pipelines';

export const pipelinesService: PipelinesService = Object.freeze({
  ...createGenericService(BASE_URL, createPipeline),
  run: async function (id: string, input: string) {
    const response = await makeRequest<Run>(`${BASE_URL}/${id}/run`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ input }),
    });

    if (response.failed) {
      return response;
    }

    return result.success(response.value);
  },
});

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createPipeline(data: any): Pipeline {
  return {
    id: data.id,
    name: data.name,
    endpoint: data.endpoint,
    description: data.description,
    createdDate: new Date(data.createdDate),
    updatedDate: new Date(data.updatedDate),
  };
}
