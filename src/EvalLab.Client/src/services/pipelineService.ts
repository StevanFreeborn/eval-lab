import { InjectionKey } from 'vue';
import { createGenericService, Entity, GenericService } from './shared';

type NewPipeline = {
  name: string;
  description: string;
  endpoint: string;
};

export type Pipeline = NewPipeline & Entity;

type PipelinesService = GenericService<NewPipeline, Pipeline>;

type PipelinesServiceKeyType = InjectionKey<PipelinesService>;

export const PipelinesServiceKey: PipelinesServiceKeyType = Symbol('PipelinesService');

const BASE_URL = '/api/pipelines';

export const pipelinesService: PipelinesService = Object.freeze(
  createGenericService(BASE_URL, createPipeline),
);

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
