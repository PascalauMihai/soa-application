<template>
  <div>
    <b-row class="text-center">
      <b-col>
        <div
          class="filter-card p-3"
          :class="{
            'bg-active-filter': true,
            'bg-light': false,
          }"
        >
          <h6>{{ totalItemsNames }}</h6>
          <h4>
            <strong>{{ totalElements }}</strong>
          </h4>
        </div>
      </b-col>
    </b-row>
  </div>
</template>

<script>
export default {
  name: "ItemsOverview",
  props: {
    totalElements: Number,
    totalItemsNames: String,
  },
};
</script>

<style>
.bg-active-filter {
  background-color: #e9f1fe;
  color: #074297;
}
</style>
